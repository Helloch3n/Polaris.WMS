using Polaris.WMS.Inventories.Reels;
using Polaris.WMS.Inventories.Transaction;
using Polaris.WMS.InventoryManage.Domain.inventories.Args;
using Polaris.WMS.InventoryManage.Domain.Integration.Locations;
using Polaris.WMS.InventoryManage.Domain.inventories;
using Polaris.WMS.MasterData.Reels;
using Polaris.WMS.MasterData.Warehouses;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Polaris.WMS.InventoryManage.Domain.Reels
{
    /// <summary>
    /// 盘具（Reel）领域服务。
    /// 封装盘具相关领域行为与业务规则（创建、更新、移库、库存归零后的盘具生命周期处理）。
    /// 注意：仅包含领域逻辑，不应泄露基础设施或 DTO 细节。
    /// </summary>
    public class ReelManager(
        IRepository<Reel, Guid> reelRepository,
        IRepository<InventoryTransaction, Guid> transactionRepository,
        IInventoryRepository inventoryRepository,
        InventoryTransactionManager inventoryTransactionManager
        //IExternalLocationAdapter externalLocationAdapter
    )
        : DomainService
    {
        private IExternalLocationProvider ExternalLocationProvider =>
            LazyServiceProvider.LazyGetRequiredService<IExternalLocationProvider>();

        /// <summary>
        /// 创建并返回新的盘具聚合。
        /// 若未提供盘号，系统将按策略生成并校验唯一性。
        /// </summary>
        /// <param name="id">盘具主键 Id。</param>
        /// <param name="reelNo">盘号（可选，空则由系统生成）。</param>
        /// <param name="name">盘具名称。</param>
        /// <param name="size">规格描述。</param>
        /// <param name="selfWeight">盘具自重。</param>
        /// <param name="currentLocationId">当前库位 Id（可空）。</param>
        /// <param name="reelType">盘具类型。</param>
        /// <returns>新建的 <see cref="Reel"/> 聚合根实例。</returns>
        public async Task<Reel> CreateAsync(
            Guid id,
            string? reelNo,
            string name,
            string size,
            decimal selfWeight,
            Guid? currentLocationId,
            ReelType reelType
        )
        {
            // 1. 盘号为空时自动生成
            var finalReelNo = string.IsNullOrWhiteSpace(reelNo)
                ? await GenerateReelNoAsync()
                : reelNo;

            // 2. 校验唯一性
            await EnsureReelNoUniqueAsync(finalReelNo);

            // 3. 构建并返回聚合
            return new Reel(
                id,
                finalReelNo,
                name,
                size,
                selfWeight,
                ReelStatus.Empty,
                currentLocationId,
                reelType);
        }

        /// <summary>
        /// 更新盘具基础属性。
        /// 若盘号变更则进行唯一性校验。具体字段变更委托实体方法执行。
        /// </summary>
        /// <param name="reel">目标盘具聚合根。</param>
        /// <param name="reelNo">更新后的盘号。</param>
        /// <param name="name">更新后的名称。</param>
        /// <param name="size">更新后的规格描述。</param>
        /// <param name="selfWeight">更新后的自重。</param>
        public async Task UpdateAsync(
            Reel reel,
            string reelNo,
            string name,
            string size,
            decimal selfWeight)
        {
            // 仅在盘号变更时校验唯一性，避免额外数据库开销
            if (!string.Equals(reel.ReelNo, reelNo, StringComparison.Ordinal))
            {
                await EnsureReelNoUniqueAsync(reelNo, reel.Id);
            }

            // 委托实体执行字段更新以保持封装
            reel.Update(reelNo, name, size, selfWeight);
        }

        /// <summary>
        /// 验证盘号唯一性。若存在重复（排除更新时自身），抛出业务异常。
        /// </summary>
        /// <param name="reelNo">待校验的盘号。</param>
        /// <param name="excludeReelId">可选：更新场景中排除自身的 Id。</param>
        public async Task EnsureReelNoUniqueAsync(string reelNo, Guid? excludeReelId = null)
        {
            var exists = await reelRepository.AnyAsync(x =>
                x.ReelNo == reelNo &&
                (!excludeReelId.HasValue || x.Id != excludeReelId.Value));

            if (exists)
            {
                throw new BusinessException("盘号已经存在")
                    .WithData("盘号", reelNo);
            }
        }

        /// <summary>
        /// 生成临时唯一盘号，策略为日期 + 随机数。若多次冲突则抛出异常。
        /// </summary>
        /// <returns>可用的盘号字符串。</returns>
        public async Task<string> GenerateReelNoAsync()
        {
            var date = Clock.Now.ToString("yyyyMMdd");

            // 最多尝试 100 次生成以避免死循环
            for (var i = 0; i < 100; i++)
            {
                var reelNo = $"R-{date}-{Random.Shared.Next(1, 1000):D3}";
                var exists = await reelRepository.AnyAsync(x => x.ReelNo == reelNo);

                if (!exists)
                {
                    return reelNo;
                }
            }

            throw new BusinessException("Reel:GenerateReelNoFailed");
        }

        /// <summary>
        /// 将盘具从当前库位搬移到目标库位（物理位移）。
        /// 不改变库存数量，但会为每条库存生成移库流水以保证可追溯性。
        /// </summary>
        /// <param name="reelId">盘具 Id。</param>
        /// <param name="targetLocationId">目标库位 Id。</param>
        /// <param name="businessOrderNo">可选的业务单号，用于流水关联。</param>
        public async Task TransferLocationAsync(Guid reelId, Guid targetLocationId, string? businessOrderNo = null)
        {
            // 1. 读取盘具聚合实体
            var reel = await reelRepository.GetAsync(reelId);

            reel.UnLock();

            // 防呆：若已在目标库位则直接返回
            if (reel.CurrentLocationId == targetLocationId)
            {
                return;
            }

            // 2. 跨聚合校验：目标库位合法性（通过外部适配器获取库位信息，避免聚合间直接导航）
            var targetLocation = await ExternalLocationProvider.GetLocationAsync(targetLocationId);

            if (targetLocation.Status == LocationStatus.Locked)
            {
                throw new BusinessException("目标库位已被锁定")
                    .WithData("LocationCode", targetLocation.Code);
            }

            if (targetLocation.Status == LocationStatus.Full)
            {
                throw new BusinessException("目标库位已满")
                    .WithData("LocationCode", targetLocation.Code);
            }

            // 可扩展点：此处可加入重量/体积等额外校验

            var originalLocationId = reel.CurrentLocationId;
            ExternalLocationInfo? sourceLocation = null;
            if (originalLocationId.HasValue)
            {
                // sourceLocation 查询通过外部适配器完成以维持上下文边界
                sourceLocation = await ExternalLocationProvider.GetLocationAsync(originalLocationId.Value);
            }

            // 3. 修改盘具的库位并持久化
            reel.SetLocation(targetLocationId);
            await reelRepository.UpdateAsync(reel);

            // 4. 刷新目标库位装载状态（通过外部适配器通知）
            await ExternalLocationProvider.RefreshStatusByLoadAsync(targetLocationId);

            // 5. 刷新原库位装载状态（如原库位存在）
            if (originalLocationId.HasValue)
            {
                await ExternalLocationProvider.RefreshStatusByLoadAsync(originalLocationId.Value);
            }

            // 6. 为盘具下每条库存生成移库流水，并保持库存的位置信息一致
            var inventories = await inventoryRepository.GetListAsync(x => x.ReelId == reelId);
            foreach (var inventory in inventories)
            {
                // 生成移库流水
                var createArgs = new CreateInventoryTranscationArgs
                {
                    Id = GuidGenerator.Create(),
                    Type = TransactionType.Transfer,
                    BillNo = string.IsNullOrWhiteSpace(businessOrderNo)
                        ? $"REEL-TRANSFER-{Clock.Now:yyyyMMddHHmmssfff}"
                        : businessOrderNo,
                    InventoryId = inventory.Id,
                    ReelId = inventory.ReelId,
                    ProductId = inventory.ProductId,
                    Quantity = inventory.Quantity,
                    QuantityAfter = inventory.Quantity,
                    FromLocationId = originalLocationId,
                    ToLocationId = targetLocationId,
                    FromWarehouseId = sourceLocation?.WarehouseId,
                    ToWarehouseId = targetLocation.WarehouseId,
                    SN = inventory.SN,
                    BatchNo = inventory.BatchNo,
                    CraftVersion = inventory.CraftVersion,
                    Status = inventory.Status,
                    Remark = "盘具移库"
                };

                await inventoryTransactionManager.CreateAsync(createArgs);
            }
        }

        /// <summary>
        /// 当盘具下无库存或全部库存为 0 时，处理盘具生命周期：
        /// - 标记为 Empty；
        /// - 若为虚拟盘具且为空，则删除并返回删除前的库位 Id。
        /// </summary>
        /// <param name="reelId">盘具 Id。</param>
        /// <returns>原始库位 Id（若删除则返回删除前的库位 Id）。</returns>
        public async Task<Guid?> HandleReelAfterInventoryDepletedAsync(Guid reelId)
        {
            var reel = await reelRepository.GetAsync(reelId);
            var locationId = reel.CurrentLocationId;

            var inventories = await inventoryRepository.GetListAsync(x => x.ReelId == reelId);

            // 盘具下无库存或数量全为 0，则标记为空
            if (inventories.Count == 0 || inventories.All(x => x.Quantity == 0))
            {
                reel.SetEmpty();
            }

            // 若为空且为虚拟盘具，则删除并返回原库位
            if (reel.Status == ReelStatus.Empty && reel.ReelType == ReelType.Virtual)
            {
                await reelRepository.DeleteAsync(reel);
                return locationId;
            }
            else
            {
                await reelRepository.UpdateAsync(reel);
            }

            return locationId;
        }
    }
}