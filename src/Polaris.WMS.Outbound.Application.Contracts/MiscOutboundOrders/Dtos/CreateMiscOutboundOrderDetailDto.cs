using System.ComponentModel.DataAnnotations;

namespace Polaris.WMS.Outbound.Application.Contracts.MiscOutboundOrders.Dtos;

public class CreateMiscOutboundOrderDetailDto
{
    public Guid WarehouseId { get; set; }

    [Required]
    [MaxLength(64)]
    public string WarehouseCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string WarehouseName { get; set; } = string.Empty;

    public Guid LocationId { get; set; }

    [Required]
    [MaxLength(64)]
    public string LocationCode { get; set; } = string.Empty;

    public Guid ContainerId { get; set; }

    [Required]
    [MaxLength(64)]
    public string ContainerCode { get; set; } = string.Empty;

    public Guid ProductId { get; set; }

    [Required]
    [MaxLength(50)]
    public string ProductCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string ProductName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string SN { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string BatchNo { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? CraftVersion { get; set; }

    [Required]
    [MaxLength(20)]
    public string Unit { get; set; } = string.Empty;

    public decimal Qty { get; set; }
    public string? Remark { get; set; }
}


