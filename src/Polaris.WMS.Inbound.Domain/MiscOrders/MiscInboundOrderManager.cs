using Polaris.WMS.Inbound.Domain.Integration.Containers;
using Polaris.WMS.Inbound.Domain.Integration.Inventories;
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

    private IExternalInventoryProvider ExternalInventoryProvider =>
        LazyServiceProvider.LazyGetRequiredService<IExternalInventoryProvider>();

    /// <summary>
    /// 审核并执行其他入库单（按容器维度入库）。
    /// </summary>
    public async Task ApproveAndExecuteAsync(Guid orderId)
    {
        var queryable = await miscInboundOrderRepository.WithDetailsAsync(x => x.Details);
        var order = await AsyncExecuter.FirstOrDefaultAsync(queryable.Where(x => x.Id == orderId));
        if (order == null)
        {
            throw new BusinessException("其他入库单不存在。")
                .WithData("OrderId", orderId);
        }

        if (!order.Details.Any())
        {
            throw new BusinessException("其他入库单至少需要一条明细，无法审核执行。")
                .WithData("OrderId", orderId);
        }

        if (order.Status != MiscOrderStatus.Draft)
        {
            throw new BusinessException("仅草稿状态单据允许审核执行。")
                .WithData("OrderId", orderId)
                .WithData("Status", order.Status);
        }

        var groupedDetails = order.Details
            .GroupBy(x => new { x.ContainerId, x.LocationId })
            .ToList();

        foreach (var group in groupedDetails)
        {
            var container = await ExternalContainerProvider.GetContainerAsync(group.Key.ContainerId);
            if (container == null)
            {
                throw new BusinessException("系统中不存在对应的容器。")
                    .WithData("ContainerId", group.Key.ContainerId);
            }

            if (container.CurrentLocationId != group.Key.LocationId)
            {
                // 隐式调拨：容器当前库位与入库目标库位不一致时先移库。
                await ExternalContainerProvider.MoveContainerAsync(group.Key.ContainerId, group.Key.LocationId, order.OrderNo);
            }

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

        order.Execute();
        await miscInboundOrderRepository.UpdateAsync(order, autoSave: true);
    }
}


