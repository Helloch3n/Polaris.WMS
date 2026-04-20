using Polaris.WMS.Inventories.Invnentory;

namespace Polaris.WMS.InventoryManage.Application.Contracts.Integration.inventories;

// 1. 物理层面的入库请求：按盘具收货
public class ProductionReceiveByContainerInput
{
    public string OrderNo { get; set; }
    public Guid ContainerId { get; set; }
    public Guid LocationId { get; set; }
    public List<ReceiveItemDto> Items { get; set; } = new();
}

// 2. 盘具上的具体物料明细
public class ReceiveItemDto
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