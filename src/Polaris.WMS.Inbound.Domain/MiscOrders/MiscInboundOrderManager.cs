using Polaris.WMS.Inbound.Domain.Integration.Containers;
using Polaris.WMS.Inbound.Domain.Integration.Inventories;
using Polaris.WMS.Inbound.Domain.Integration.MasterData;
using Polaris.WMS.Inventories.Invnentory;
using Polaris.WMS.Inventories.MiscOrders;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Polaris.WMS.Inbound.Domain.MiscOrders;

/// <summary>
/// 其他入库单领域服务，负责审核执行与库存落账编排。
/// </summary>
public class MiscInboundOrderManager(
    IRepository<MiscInboundOrder, Guid> miscInboundOrderRepository)
    : DomainService
{
    private IExternalContainerProvider ExternalContainerProvider =>
        LazyServiceProvider.LazyGetRequiredService<IExternalContainerProvider>();

    private ILocationAllocationProvider LocationAllocationProvider =>
        LazyServiceProvider.LazyGetRequiredService<ILocationAllocationProvider>();

    private IProductValidationProvider ProductValidationProvider =>
        LazyServiceProvider.LazyGetRequiredService<IProductValidationProvider>();

    private IExternalInventoryProvider ExternalInventoryProvider =>
        LazyServiceProvider.LazyGetRequiredService<IExternalInventoryProvider>();

    /// <summary>
    /// 审核并执行其他入库单（按容器维度入库）。
    /// </summary>
    public async Task ApproveAndExecuteAsync(Guid orderId)
    {
        // 1. 加载单据及明细
        var queryable = await miscInboundOrderRepository.WithDetailsAsync(x => x.Details);
        var order = await AsyncExecuter.FirstOrDefaultAsync(queryable.Where(x => x.Id == orderId));
        if (order == null)
        {
            throw new BusinessException("其他入库单不存在。")
                .WithData("OrderId", orderId);
        }

        // 2. 校验单据存在有效明细
        if (!order.Details.Any())
        {
            throw new BusinessException("其他入库单至少需要一条明细，无法审核执行。")
                .WithData("OrderId", orderId);
        }

        // 3. 校验草稿状态
        if (order.Status != MiscOrderStatus.Draft)
        {
            throw new BusinessException("仅草稿状态单据允许审核执行。")
                .WithData("OrderId", orderId)
                .WithData("Status", order.Status);
        }

        // 4. 校验明细业务字段
        foreach (var detail in order.Details)
        {
            if (detail.Qty <= 0)
            {
                throw new BusinessException("其他入库明细数量必须大于0，无法审核执行。")
                    .WithData("OrderId", orderId)
                    .WithData("DetailId", detail.Id)
                    .WithData("Qty", detail.Qty);
            }

            if (detail.ProductId == Guid.Empty)
            {
                throw new BusinessException("其他入库明细的物料Id不能为空。")
                    .WithData("OrderId", orderId)
                    .WithData("DetailId", detail.Id);
            }

            if (!await ProductValidationProvider.ExistsAsync(detail.ProductId))
            {
                throw new BusinessException("其他入库明细的物料不存在或尚未同步，无法审核执行。")
                    .WithData("OrderId", orderId)
                    .WithData("DetailId", detail.Id)
                    .WithData("ProductId", detail.ProductId);
            }

            if (detail.ContainerId == Guid.Empty)
            {
                throw new BusinessException("其他入库明细的容器Id不能为空。")
                    .WithData("OrderId", orderId)
                    .WithData("DetailId", detail.Id);
            }

            if (detail.LocationId == Guid.Empty)
            {
                throw new BusinessException("其他入库明细的库位Id不能为空。")
                    .WithData("OrderId", orderId)
                    .WithData("DetailId", detail.Id);
            }
        }

        // 5. 按容器 + 库位分组执行
        var groupedDetails = order.Details
            .GroupBy(x => new { x.ContainerId, x.LocationId })
            .ToList();

        foreach (var group in groupedDetails)
        {
            // 5.1 校验目标库位是否锁定
            if (await LocationAllocationProvider.IsLockedAsync(group.Key.LocationId))
            {
                throw new BusinessException("目标库位已锁定，无法执行其他入库。")
                    .WithData("OrderId", orderId)
                    .WithData("LocationId", group.Key.LocationId);
            }

            // 5.2 校验容器存在
            var container = await ExternalContainerProvider.GetContainerAsync(group.Key.ContainerId);
            if (container == null)
            {
                throw new BusinessException("系统中不存在对应的容器。")
                    .WithData("ContainerId", group.Key.ContainerId);
            }

            // 5.3 隐式调拨：容器当前库位与入库目标库位不一致时先移库
            if (container.CurrentLocationId != group.Key.LocationId)
            {
                await ExternalContainerProvider.MoveContainerAsync(group.Key.ContainerId, group.Key.LocationId, order.OrderNo);
            }

            // 5.4 调用库存服务执行按容器入库
            var receiveInfo = new ExternalProductionReceiveInfo
            {
                OrderNo = order.OrderNo,
                ReelId = group.Key.ContainerId,
                ActualLocationId = group.Key.LocationId,
                Items = group.Select(x => new ExternalProductionReceiveInfoItem
                {
                    ProductId = x.ProductId,
                    Qty = x.Qty,
                    Weight = 0,
                    BatchNo = x.BatchNo,
                    RelatedOrderNo = order.OrderNo,
                    RelatedOrderNoLineNo = x.Id.ToString("N"),
                    SN = x.SN,
                    Unit = x.Unit,
                    CraftVersion = x.CraftVersion ?? string.Empty,
                    LayerIndex = 0,
                    Status = InventoryStatus.Hold
                }).ToList()
            };

            await ExternalInventoryProvider.ReceiveProductionAsync(receiveInfo);
        }

        // 6. 单据置为已执行并持久化
        order.Execute();
        await miscInboundOrderRepository.UpdateAsync(order, autoSave: true);
    }
}


