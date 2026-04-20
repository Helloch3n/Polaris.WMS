using System.ComponentModel.DataAnnotations;

namespace Polaris.WMS.Outbound.Application.Contracts.MiscOutboundOrders.Dtos;

public class UpdateMiscOutboundOrderDto
{
    public Guid AccountAliasId { get; set; }

    [Required]
    [MaxLength(500)]
    public string AccountAliasDescription { get; set; } = string.Empty;

    public Guid CostCenterId { get; set; }

    [Required]
    [MaxLength(50)]
    public string CostCenterCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string CostCenterName { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Remark { get; set; }

    public List<CreateMiscOutboundOrderDetailDto> Details { get; set; } = new();
}

