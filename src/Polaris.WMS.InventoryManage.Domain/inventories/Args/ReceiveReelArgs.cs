using Polaris.WMS.Inventories.Invnentory;

namespace Polaris.WMS.InventoryManage.Domain.inventories.Args;

/// <summary>
/// 领域层使用的入库参数对象
/// </summary>
public class ReceiveReelArgs
{
    public string OrderNo { get; set; }
    public Guid ReelId { get; set; }
    public string ReelCode { get; set; }
    public Guid LocationId { get; set; }
    public List<ReceiveReelItemArgs> Items { get; set; } = new();
}

public class ReceiveReelItemArgs
{
    public Guid ProductId { get; set; }
    public decimal Qty { get; set; }
    public decimal Weight { get; set; }
    public string BatchNo { get; set; }
    public string RelatedOrderNo { get; set; }
    public string RelatedOrderLineNo { get; set; }
    public string SN { get; set; }
    public string Unit { get; set; }
    public string CraftVersion { get; set; }
    public int LayerIndex { get; set; }
    public InventoryStatus Status { get; set; }
}