using Polaris.WMS.PurchaseOrders;
using Volo.Abp.Application.Dtos;

namespace Polaris.WMS.Inbound.Application.Contracts.PurchaseOrders.Dtos;

public class GetPurchaseOrderListDto : PagedAndSortedResultRequestDto
{
    public string Filter { get; set; }
    public PurchaseOrderStatus? Status { get; set; }
}