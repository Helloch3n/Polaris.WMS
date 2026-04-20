using Polaris.WMS.Inventories.MiscOrders;
using Volo.Abp.Application.Dtos;

namespace Polaris.WMS.Outbound.Application.Contracts.MiscOutboundOrders.Dtos;

public class MiscOutboundOrderDto : AuditedEntityDto<Guid>
{
    public string OrderNo { get; set; } = string.Empty;
    public Guid AccountAliasId { get; set; }
    public string AccountAliasDescription { get; set; } = string.Empty;
    public Guid CostCenterId { get; set; }
    public string CostCenterCode { get; set; } = string.Empty;
    public string CostCenterName { get; set; } = string.Empty;
    public MiscOperationType Type { get; set; }
    public MiscOrderStatus Status { get; set; }
    public string? Remark { get; set; }
    public List<MiscOutboundOrderDetailDto> Details { get; set; } = new();
}

