using Polaris.WMS.BillNumbers;
using Polaris.WMS.Inbound.Domain.Integration.Inventories;
using Polaris.WMS.Inbound.Domain.Integration.Reels;
using Polaris.WMS.Inventorys;
using Polaris.WMS.ProductionInbounds;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Polaris.WMS.Inbound.Domain.ProductionInbounds
{
    public class ProductionInboundManager(
        IRepository<ProductionInbound, Guid> productionInboundRepository,
        IBillNumberGenerator billNumberGenerator
        //IExternalReelProvider externalReelProvider,
        //IExternalInventoryAdapter externalInventoryAdapter
    )
        : DomainService
    {
        private IExternalReelProvider ExternalReelProvider =>
            LazyServiceProvider.LazyGetRequiredService<IExternalReelProvider>();

        private IExternalInventoryAdapter ExternalInventoryAdapter =>
            LazyServiceProvider.LazyGetRequiredService<IExternalInventoryAdapter>();

        /// <summary>
        /// 创建生产入库单聚合（仅构建实体，不持久化）。
        /// </summary>
        public async Task<ProductionInbound> CreateAsync(
            Guid id,
            string sourceOrderNo,
            Guid sourceDepartmentId,
            Guid targetWarehouseId,
            ProductionInboundType productionInboundType)
        {
            if (sourceDepartmentId == Guid.Empty || targetWarehouseId == Guid.Empty)
            {
                throw new BusinessException("来源车间和目标仓库不能为空！");
            }

            // 1. 生成流水号，前缀为 PIN (Production Inbound)
            var orderNo = await billNumberGenerator.GetNextNumberAsync("PIN");
            var normalizedOrderNo = Check.NotNullOrWhiteSpace(orderNo, nameof(orderNo));

            // 2. 校验单号防重
            var exists = await productionInboundRepository.AnyAsync(x => x.OrderNo == normalizedOrderNo);
            if (exists)
            {
                throw new BusinessException("生产入库单号已存在")
                    .WithData("单号", normalizedOrderNo);
            }

            // 3. 调用实体内部的静态工厂方法，确保业务不变量（如 DepartmentId = sourceDepartmentId）
            return ProductionInbound.Create(
                id,
                normalizedOrderNo,
                sourceOrderNo,
                sourceDepartmentId,
                targetWarehouseId,
                productionInboundType // 默认类型为成品入库，可根据实际需求调整
            );
        }

        /// <summary>
        /// 执行单盘入库
        /// </summary>
        /// <param name="orderId">入库单 ID</param>
        /// <param name="reelNo">扫码获取的盘具编号</param>
        /// <param name="actualLocationId">实际放入的库位（暂存区或正式区）</param>
        public async Task ReceiveReelAsync(Guid orderId, Guid reelId, Guid actualLocationId)
        {
            // 1. 加载入库单及明细
            var query = await productionInboundRepository.WithDetailsAsync(x => x.Details);
            var order = await AsyncExecuter.FirstOrDefaultAsync(query.Where(x => x.Id == orderId));

            if (order == null)
            {
                throw new BusinessException("生产入库单不存在").WithData("OrderId", orderId);
            }

            if (order.Status != ProductionInboundStatus.Draft)
            {
                throw new BusinessException("只有草稿状态才能审核执行！");
            }

            if (order.Status == ProductionInboundStatus.Completed)
            {
                throw new BusinessException("该入库单已全部完成，无法继续入库！");
            }

            // 2. 获取盘具实体纠正盘具库位

            var reel = await ExternalReelProvider.GetReelAsync(reelId);

            if (reel == null)
            {
                throw new BusinessException($"系统中不存在 ID 为 {reelId} 的盘具！");
            }

            if (reel.CurrentLocationId != actualLocationId)
            {
                // 调用盘具领域服务，执行一次隐式调拨/移库
                await ExternalReelProvider.MoveReelAsync(
                    reel.Id,
                    actualLocationId,
                    order.OrderNo
                );
            }

            // 3. 查找该盘具对应的所有未入库明细（同盘必同绑）
            var pendingDetails = order.Details
                .Where(x => x.ReelId == reelId && x.Status == ProductionInboundDetailStatus.Pending)
                .ToList();

            if (!pendingDetails.Any())
            {
                throw new BusinessException($"单据中未找到盘号为 {reelId} 的待入库明细！");
            }

            // 4. 标记明细状态并记录实际库位
            foreach (var detail in pendingDetails)
            {
                // 标记明细为已完成（已入库）
                order.MarkDetailAsCompleted(detail.Id);
                // 调用库存领域服务，创建实物库存
                var receiveInfo = new ExternalProductionReceiveInfo
                {
                    OrderNo = order.OrderNo,
                    ReelId = detail.ReelId,
                    ProductId = detail.ProductId,
                    Qty = detail.Qty,
                    Weight = detail.Weight,
                    BatchNo = detail.BatchNo,
                    RelatedOrderNo = detail.RelatedOrderNo,
                    RelatedOrderNoLineNo = detail.RelatedOrderNoLineNo,
                    ActualLocationId = detail.ActualLocationId,
                    SN = detail.SN,
                    Unit = detail.Unit,
                    CraftVersion = detail.CraftVersion,
                    LayerIndex = detail.LayerIndex,
                    Status = InventoryStatus.Quarantine // 隔离状态
                };
                await ExternalInventoryAdapter.ReceiveProductionAsync(receiveInfo);
            }

            // 5. 持久化最新状态
            await productionInboundRepository.UpdateAsync(order);
        }

        /// <summary>
        /// 删除生产入库单（仅允许草稿状态）。
        /// </summary>
        public async Task DeleteAsync(Guid orderId)
        {
            // 1. 加载入库单
            var query = await productionInboundRepository.WithDetailsAsync(x => x.Details);
            var order = await AsyncExecuter.FirstOrDefaultAsync(query.Where(x => x.Id == orderId));
            if (order == null)
            {
                throw new BusinessException("生产入库单不存在").WithData("OrderId", orderId);
            }

            // 2. 仅允许删除草稿单（一旦进入 InProgress，说明有实物入库了，严禁删除，只能红冲）
            if (order.Status != ProductionInboundStatus.Draft)
            {
                throw new BusinessException("只有草稿状态的入库单允许删除！如果实物已入库，请执行退料或红冲流程。");
            }

            // 3. 删除入库单（ABP 会自动处理软删除，且级联删除明细）
            await productionInboundRepository.DeleteAsync(order, autoSave: true);
        }
    }
}