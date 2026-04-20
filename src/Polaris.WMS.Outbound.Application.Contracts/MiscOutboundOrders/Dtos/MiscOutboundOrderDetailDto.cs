using Volo.Abp.Application.Dtos;

namespace Polaris.WMS.Outbound.Application.Contracts.MiscOutboundOrders.Dtos;

public class MiscOutboundOrderDetailDto : EntityDto<Guid>
{
    public Guid WarehouseId { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public Guid LocationId { get; set; }
    public string LocationCode { get; set; } = string.Empty;
    public Guid ContainerId { get; set; }
    public string ContainerCode { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string SN { get; set; } = string.Empty;
    public string BatchNo { get; set; } = string.Empty;
    public string? CraftVersion { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal Qty { get; set; }
    public string? Remark { get; set; }
}


