using Polaris.WMS.Inventories.MiscOrders;
using Volo.Abp.Application.Dtos;

namespace Polaris.WMS.Outbound.Application.Contracts.MiscOutboundOrders.Dtos;

public class MiscOutboundOrderSearchDto : PagedAndSortedResultRequestDto
{
    public string? OrderNo { get; set; }
    public MiscOrderStatus? Status { get; set; }
}

