using Polaris.WMS.PurchaseOrders;
using Volo.Abp.Application.Dtos;

namespace Polaris.WMS.Inbound.Application.Contracts.PurchaseOrders.Dtos;

public class PurchaseOrderDto : AuditedEntityDto<Guid>
{
    public string PoNo { get; set; }
    public Guid SupplierId { get; set; }
    public string SupplierCode { get; set; }
    public string SupplierName { get; set; }
    public PurchaseOrderStatus Status { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    
    public List<PurchaseOrderDetailDto> Details { get; set; } = new();
}