using Polaris.WMS.Inventories.Invnentory;
using Polaris.WMS.Inventories.Transaction;

namespace Polaris.WMS.InventoryManage.Domain.inventories.Args;

public class CreateInventoryTranscationArgs
{
    public Guid Id { get; set; }
    public TransactionType Type { get; set; }

    public string BillNo { get; set; }
    public Guid InventoryId { get; set; }

    public Guid ContainerId { get; set; }
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }

    public decimal QuantityAfter { get; set; }
    public Guid? FromLocationId { get; set; }

    public Guid? ToLocationId { get; set; }
    public Guid? FromWarehouseId { get; set; }
    public Guid? ToWarehouseId { get; set; }
    public string? SN { get; set; }
    public string? BatchNo { get; set; }

    public string? CraftVersion { get; set; }
    public InventoryStatus Status { get; set; }
    public string? Remark { get; set; } = null;
}