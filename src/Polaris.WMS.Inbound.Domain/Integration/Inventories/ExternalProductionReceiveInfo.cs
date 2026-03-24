using Polaris.WMS.Inventorys;

namespace Polaris.WMS.Inbound.Domain.Integration.Inventories;

public class ExternalProductionReceiveInfo
{
    public string OrderNo { get; set; }
    public Guid ReelId { get; set; }
    public Guid ProductId { get; set; }
    public decimal Qty { get; set; }
    public decimal Weight { get; set; }
    public string BatchNo { get; set; }
    public string RelatedOrderNo { get; set; }
    public string RelatedOrderNoLineNo { get; set; }
    public Guid ActualLocationId { get; set; }
    public string SN { get; set; }
    public string Unit { get; set; }
    public string CraftVersion { get; set; }
    public int LayerIndex { get; set; }
    public InventoryStatus Status { get; set; }
}