using System;
using Volo.Abp.EventBus;

namespace Polaris.WMS.Inbound.Events;

/// <summary>
/// 采购收货明细执行完成后发布的本地事件。
/// </summary>
[EventName("Polaris.WMS.Inbound.PurchaseReceipt.Executed")]
public class PurchaseReceiptExecutedLocalEto
{
    public Guid? SourceAsnLineId { get; set; }

    public Guid? SourcePoLineId { get; set; }

    public Guid ReceiptOrderId { get; set; }

    public Guid ProductId { get; set; }

    public decimal Quantity { get; set; }

    public Guid ReceiptOrderLineId { get; set; }
}


