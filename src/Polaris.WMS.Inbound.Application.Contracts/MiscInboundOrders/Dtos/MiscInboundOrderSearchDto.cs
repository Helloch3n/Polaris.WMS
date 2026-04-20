using Polaris.WMS.Inventories.MiscOrders;
using Volo.Abp.Application.Dtos;

namespace Polaris.WMS.Inbound.Application.Contracts.MiscInboundOrders.Dtos;

public class MiscInboundOrderSearchDto : PagedAndSortedResultRequestDto
{
    public string? OrderNo { get; set; }
    public MiscOrderStatus? Status { get; set; }
}

