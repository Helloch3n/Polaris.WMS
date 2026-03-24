using Polaris.WMS.Inventorys;

namespace Polaris.WMS.InventoryManage.Application.Contracts.InventoryTransactions.Dtos
{
    public class InventoryTransactionDto
    {
        public Guid Id { get; set; }
        public DateTime CreationTime { get; set; }
        public Guid? CreatorId { get; set; }
        public TransactionType Type { get; set; }

        public string BillNo { get; set; }
        public string Remark { get; set; }

        // 关联
        public Guid ReelId { get; set; }
        public string ReelNo { get; set; }

        public Guid ProductId { get; set; }
        public string ProductName { get; set; }

        // 库位 (From / To，可能为空)
        public Guid? FromLocationId { get; set; }
        public string FromLocationCode { get; set; }
        public Guid? ToLocationId { get; set; }
        public string ToLocationCode { get; set; }

        // 仓库 (From / To，可能为空)
        public Guid? FromWarehouseId { get; set; }
        public string FromWarehouseCode { get; set; }
        public Guid? ToWarehouseId { get; set; }
        public string ToWarehouseCode { get; set; }

        // 数量
        public decimal Quantity { get; set; }
        public decimal QuantityAfter { get; set; }

        // 追溯快照
        public string BatchNo { get; set; }
        public string SN { get; set; }
        public string CraftVersion { get; set; }
        public InventoryStatus Status { get; set; }
    }
}