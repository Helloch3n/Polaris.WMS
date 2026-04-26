namespace Polaris.WMS.Outbound.Domain.Integration.Inventories;

public class ExternalMiscOutboundShipInfo
{
    public string OrderNo { get; set; } = string.Empty;
    public Guid DetailId { get; set; }
    public Guid ContainerId { get; set; }
    public Guid LocationId { get; set; }
    public Guid ProductId { get; set; }
    public decimal Qty { get; set; }
    public string BatchNo { get; set; } = string.Empty;
    public string SN { get; set; } = string.Empty;
}

