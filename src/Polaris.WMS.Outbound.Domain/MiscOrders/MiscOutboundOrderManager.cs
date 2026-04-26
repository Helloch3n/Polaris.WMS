using Polaris.WMS.Inventories.MiscOrders;
using Polaris.WMS.Outbound.Domain.Integration.Inventories;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Polaris.WMS.Outbound.Domain.MiscOrders;

/// <summary>
/// 其他出库单领域服务，负责审核执行与库存扣减编排。
/// </summary>
public class MiscOutboundOrderManager(
    IRepository<MiscOutboundOrder, Guid> miscOutboundOrderRepository)
    : DomainService
{
    private IExternalInventoryProvider ExternalInventoryProvider =>
        LazyServiceProvider.LazyGetRequiredService<IExternalInventoryProvider>();

    /// <summary>
    /// 审核并执行其他出库单。
    /// </summary>
    public async Task ApproveAndExecuteAsync(Guid orderId)
    {
        // 1. 加载单据及明细
        var queryable = await miscOutboundOrderRepository.WithDetailsAsync(x => x.Details);
        var order = await AsyncExecuter.FirstOrDefaultAsync(queryable.Where(x => x.Id == orderId));
        if (order == null)
        {
            throw new BusinessException("其他出库单不存在。")
                .WithData("OrderId", orderId);
        }

        // 2. 校验明细存在
        if (!order.Details.Any())
        {
            throw new BusinessException("其他出库单至少需要一条明细，无法审核执行。")
                .WithData("OrderId", orderId);
        }

        // 3. 校验草稿状态
        if (order.Status != MiscOrderStatus.Draft)
        {
            throw new BusinessException("仅草稿状态单据允许审核执行。")
                .WithData("OrderId", orderId)
                .WithData("Status", order.Status);
        }

        // 4. 校验明细数量并执行出库
        foreach (var detail in order.Details)
        {
            if (detail.Qty <= 0)
            {
                throw new BusinessException("其他出库明细数量必须大于0，无法审核执行。")
                    .WithData("OrderId", orderId)
                    .WithData("DetailId", detail.Id)
                    .WithData("Qty", detail.Qty);
            }

            await ExternalInventoryProvider.ShipAsync(new ExternalMiscOutboundShipInfo
            {
                OrderNo = order.OrderNo,
                DetailId = detail.Id,
                ContainerId = detail.ContainerId,
                LocationId = detail.LocationId,
                ProductId = detail.ProductId,
                Qty = detail.Qty,
                BatchNo = detail.BatchNo,
                SN = detail.SN
            });
        }

        // 5. 单据执行完成
        order.Execute();
        await miscOutboundOrderRepository.UpdateAsync(order, autoSave: true);
    }
}

