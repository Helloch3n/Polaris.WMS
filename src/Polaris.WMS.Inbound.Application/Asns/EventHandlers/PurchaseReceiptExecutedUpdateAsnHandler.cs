using Polaris.WMS.Inbound.Domain.Asns;
using Polaris.WMS.Inbound.Events;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus;
using Volo.Abp.Linq;

namespace Polaris.WMS.Inbound.Application.Asns.EventHandlers;

/// <summary>
/// 处理采购收货执行成功事件，并回写 ASN 明细接收数量。
/// </summary>
public class PurchaseReceiptExecutedUpdateAsnHandler(
    IRepository<AdvancedShippingNotice, Guid> asnRepository,
    IAsyncQueryableExecuter asyncQueryableExecuter)
    : ILocalEventHandler<PurchaseReceiptExecutedLocalEto>, ITransientDependency
{
    public async Task HandleEventAsync(PurchaseReceiptExecutedLocalEto eventData)
    {
        if (!eventData.SourceAsnLineId.HasValue)
        {
            return;
        }

        if (eventData.Quantity <= 0)
        {
            throw new BusinessException("收货数量必须大于0，无法回写ASN明细。")
                .WithData("Quantity", eventData.Quantity)
                .WithData("ReceiptOrderLineId", eventData.ReceiptOrderLineId);
        }

        var queryable = await asnRepository.WithDetailsAsync(x => x.Details);
        var asn = await asyncQueryableExecuter.FirstOrDefaultAsync(
            queryable.Where(x => x.Details.Any(d => d.Id == eventData.SourceAsnLineId.Value)));

        if (asn == null)
        {
            throw new BusinessException("未找到来源ASN明细，无法回写接收数量。")
                .WithData("SourceAsnLineId", eventData.SourceAsnLineId.Value)
                .WithData("ReceiptOrderLineId", eventData.ReceiptOrderLineId);
        }

        asn.AddReceivedQty(eventData.SourceAsnLineId.Value, eventData.Quantity);
        await asnRepository.UpdateAsync(asn, autoSave: true);
    }
}



