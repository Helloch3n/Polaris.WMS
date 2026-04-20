using Polaris.WMS.Inbound.Domain.PurchaseOrders;
using Polaris.WMS.Inbound.Events;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus;
using Volo.Abp.Linq;

namespace Polaris.WMS.Inbound.Application.PurchaseOrders.EventHandlers;

/// <summary>
/// 处理采购收货执行成功事件，并回写采购单明细接收数量。
/// </summary>
public class PurchaseReceiptExecutedUpdatePoHandler(
    IRepository<PurchaseOrder, Guid> purchaseOrderRepository,
    IAsyncQueryableExecuter asyncQueryableExecuter)
    : ILocalEventHandler<PurchaseReceiptExecutedLocalEto>, ITransientDependency
{
    public async Task HandleEventAsync(PurchaseReceiptExecutedLocalEto eventData)
    {
        if (!eventData.SourcePoLineId.HasValue)
        {
            return;
        }

        if (eventData.Quantity <= 0)
        {
            throw new BusinessException("收货数量必须大于0，无法回写采购单明细。")
                .WithData("Quantity", eventData.Quantity)
                .WithData("ReceiptOrderLineId", eventData.ReceiptOrderLineId);
        }

        var queryable = await purchaseOrderRepository.WithDetailsAsync(x => x.Details);
        var purchaseOrder = await asyncQueryableExecuter.FirstOrDefaultAsync(
            queryable.Where(x => x.Details.Any(d => d.Id == eventData.SourcePoLineId.Value)));

        if (purchaseOrder == null)
        {
            throw new BusinessException("未找到来源采购单明细，无法回写接收数量。")
                .WithData("SourcePoLineId", eventData.SourcePoLineId.Value)
                .WithData("ReceiptOrderLineId", eventData.ReceiptOrderLineId);
        }

        purchaseOrder.AddReceivedQty(eventData.SourcePoLineId.Value, eventData.Quantity);
        await purchaseOrderRepository.UpdateAsync(purchaseOrder, autoSave: true);
    }
}


