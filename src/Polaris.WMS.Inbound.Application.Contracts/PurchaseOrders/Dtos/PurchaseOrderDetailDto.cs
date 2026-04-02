using Volo.Abp.Application.Dtos;

namespace Polaris.WMS.Inbound.Application.Contracts.PurchaseOrders.Dtos;

public class PurchaseOrderDetailDto : EntityDto<Guid>
{
    public Guid PurchaseOrderId { get; set; }
    public int LineNo { get; set; }
    public Guid ProductId { get; set; }
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public string UoM { get; set; }
    public decimal ExpectedQty { get; set; }
    public decimal ReceivedQty { get; set; }
    public decimal DeliveredQty { get; set; }
    public bool IsQualityCheckRequired { get; set; }
}