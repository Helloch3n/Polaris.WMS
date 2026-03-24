using Polaris.WMS.InventoryManage.Domain.Integration.Locations;
using Polaris.WMS.InventoryManage.Domain.inventories;
using Polaris.WMS.Inventorys;
using Polaris.WMS.MasterData.Reels;
using Polaris.WMS.MasterData.Warehouses;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Polaris.WMS.InventoryManage.Domain.Reels
{
    /// <summary>
    /// 盘具领域服务，负责盘具创建与盘号唯一性相关业务规则。
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
        private IExternalLocationAdapter ExternalLocationAdapter => LazyServiceProvider.LazyGetRequiredService<IExternalLocationAdapter>();
        /// <summary>
        /// 创建盘具聚合，并在盘号为空时自动生成盘号。
        /// </summary>
        /// <param name="id">盘具主键。</param>
        /// <param name="reelNo">盘号（可为空）。</param>
        /// <param name="name">盘具名称。</param>
        /// <param name="size">规格。</param>
        /// <param name="selfWeight">自重。</param>
        /// <param name="currentLocationId">当前库位。</param>
        /// <returns>创建完成的盘具聚合。</returns>
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
            // 1. 盘号为空时自动生成盘号
            var finalReelNo = string.IsNullOrWhiteSpace(reelNo)
                ? await GenerateReelNoAsync()
                : reelNo;

            // 2. 保证盘号唯一
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
        /// 更新盘具基础信息，并在盘号变更时校验唯一性。
        /// </summary>
        /// <param name="reel">盘具聚合。</param>
        /// <param name="reelNo">盘号。</param>
        /// <param name="name">盘具名称。</param>
        /// <param name="size">规格。</param>
        /// <param name="selfWeight">自重。</param>
        public async Task UpdateAsync(
            Reel reel,
            string reelNo,
            string name,
            string size,
            decimal selfWeight)
        {
            // 仅在盘号变更时执行唯一性校验
            if (!string.Equals(reel.ReelNo, reelNo, StringComparison.Ordinal))
            {
                await EnsureReelNoUniqueAsync(reelNo, reel.Id);
            }

            reel.Update(reelNo, name, size, selfWeight);
        }

        /// <summary>
        /// 校验盘号唯一性。
        /// </summary>
        /// <param name="reelNo">待校验盘号。</param>
        /// <param name="excludeReelId">排除的盘具主键（用于更新场景）。</param>
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
        /// 生成唯一盘号。
        /// </summary>
        /// <returns>可用盘号。</returns>
        public async Task<string> GenerateReelNoAsync()
        {
            var date = Clock.Now.ToString("yyyyMMdd");

            // 最多尝试 100 次随机生成
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
        /// 盘具移库 (物理位置转移，库存数量不变)
        /// </summary>
        /// <param name="reelId">盘具ID</param>
        /// <param name="targetLocationId">目标库位ID</param>
        /// <param name="businessOrderNo">触发移库的业务单号(可选，用于追溯)</param>
        public async Task TransferLocationAsync(Guid reelId, Guid targetLocationId, string? businessOrderNo = null)
        {
            // 1. 获取盘具实体
            var reel = await reelRepository.GetAsync(reelId);

            // 防呆校验：如果已经在目标库位，直接忽略
            if (reel.CurrentLocationId == targetLocationId)
            {
                return;
            }

            // 2. 跨聚合校验：目标库位合法性
            //var targetLocation = await _locationRepository.GetAsync(targetLocationId);
            var targetLocation = await ExternalLocationAdapter.GetLocationAsync(targetLocationId);

            if (targetLocation.Status == LocationStatus.Locked)
            {
                throw new BusinessException("，目标库位已被锁定")
                    .WithData("LocationCode", targetLocation.Code);
            }

            if (targetLocation.Status == LocationStatus.Full)
            {
                throw new BusinessException("，目标库位已满")
                    .WithData("LocationCode", targetLocation.Code);
            }

            // TODO: 如果未来需要重量校验，可在此处扩展
            // var totalWeight = reel.SelfWeight + await CalculateInventoryWeightAsync(reelId);
            // if (targetLocation.MaxWeight < totalWeight) throw ...

            var originalLocationId = reel.CurrentLocationId;
            //Location? sourceLocation = null;
            ExternalLocationInfo? sourceLocation = null;
            if (originalLocationId.HasValue)
            {
                //sourceLocation = await _locationRepository.FirstOrDefaultAsync(x => x.Id == originalLocationId.Value);
                sourceLocation = await ExternalLocationAdapter.GetLocationAsync(originalLocationId.Value);
            }

            // 3. 改变盘具自身的物理位置
            reel.SetLocation(targetLocationId);
            await reelRepository.UpdateAsync(reel, autoSave: true);

            // 4. 刷新目标库位状态
            //await _locationManager.RefreshStatusByLoadAsync(targetLocationId);
            await ExternalLocationAdapter.RefreshStatusByLoadAsync(targetLocationId);

            // 5. 刷新原库位状态
            if (originalLocationId.HasValue)
            {
                //await _locationManager.RefreshStatusByLoadAsync(originalLocationId.Value);
                await ExternalLocationAdapter.RefreshStatusByLoadAsync(originalLocationId.Value);
            }

            // 6. 核心动作：生成库存流水 & 同步库存明细的位置
            // 因为盘具移动了，上面的所有库存都被“打包”移动了
            var inventories = await inventoryRepository.GetListAsync(x => x.ReelId == reelId);
            foreach (var inventory in inventories)
            {
                // 生成移库流水
                var transaction = await inventoryTransactionManager.CreateAsync(
                    id: GuidGenerator.Create(),
                    type: TransactionType.Transfer,
                    billNo: string.IsNullOrWhiteSpace(businessOrderNo)
                        ? $"REEL-TRANSFER-{Clock.Now:yyyyMMddHHmmssfff}"
                        : businessOrderNo,
                    inventoryId: inventory.Id,
                    reelId: inventory.ReelId,
                    productId: inventory.ProductId,
                    quantity: inventory.Quantity,
                    quantityAfter: inventory.Quantity,
                    fromLocationId: originalLocationId,
                    toLocationId: targetLocationId,
                    fromWarehouseId: sourceLocation?.WarehouseId,
                    toWarehouseId: targetLocation.WarehouseId,
                    sn: inventory.SN,
                    batchNo: inventory.BatchNo,
                    craftVersion: inventory.CraftVersion,
                    status: inventory.Status,
                    remark: "盘具移库"
                );


                await transactionRepository.InsertAsync(transaction);
            }
        }

        /// <summary>
        /// 处理盘具在库存归零场景下的生命周期，并返回盘具原库位。
        /// </summary>
        /// <param name="reelId">盘具Id。</param>
        /// <returns>盘具原库位Id。</returns>
        public async Task<Guid?> HandleReelAfterInventoryDepletedAsync(Guid reelId)
        {
            var reel = await reelRepository.GetAsync(reelId);
            var locationId = reel.CurrentLocationId;

            var inventoryCount = await inventoryRepository.CountAsync(x => x.ReelId == reelId);

            // 第一部分：盘具下已无库存时，盘具状态置空
            if (inventoryCount == 0)
            {
                reel.SetEmpty();
            }

            // 第二部分：同时满足 Empty + Virtual 时，硬删除盘具
            if (reel.Status == ReelStatus.Empty && reel.ReelType == ReelType.Virtual)
            {
                await reelRepository.DeleteAsync(reel);
                return locationId;
            }

            if (inventoryCount == 0)
            {
                await reelRepository.UpdateAsync(reel);
            }

            return locationId;
        }
    }
}