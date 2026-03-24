using Polaris.WMS.InventoryManage.Domain.Reels;
using Polaris.WMS.Inventorys;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Polaris.WMS.InventoryManage.Domain.inventories
{
    public class InventoryTransaction : CreationAuditedEntity<Guid>
    {
        // ==========================================
        // 1. 业务核心 (Business Context)
        // ==========================================
        public string BillNo { get; private set; }
        public TransactionType Type { get; private set; }
        public string Remark { get; private set; }

        // ==========================================
        // 2. 关联对象 (Relations)
        // ==========================================
        public Guid InventoryId { get; private set; }
        public Guid ReelId { get; private set; }
        public Guid ProductId { get; private set; }

        // ==========================================
        // 3. 位置变动 (Location Change - Single Entry)
        // ==========================================
        public Guid? FromLocationId { get; private set; }
        public Guid? ToLocationId { get; private set; }

        public Guid? FromWarehouseId { get; private set; }
        public Guid? ToWarehouseId { get; private set; }

        // ==========================================
        // 4. 数量逻辑 (Quantity)
        // ==========================================
        /// <summary>
        /// 操作数量 (绝对值，始终为正)
        /// </summary>
        public decimal Quantity { get; private set; }

        /// <summary>
        /// 变动后结存
        /// </summary>
        public decimal QuantityAfter { get; private set; }

        // ==========================================
        // 5. 追溯快照 (Traceability Snapshot)
        // ==========================================
        public string BatchNo { get; private set; }
        public string SN { get; private set; }
        public string CraftVersion { get; private set; }
        public InventoryStatus Status { get; private set; }

        // ==========================================
        // 6. 导航属性 (Navigation Properties)
        // ==========================================
        public Reel Reel { get; private set; }

        protected InventoryTransaction()
        {
        }

        internal InventoryTransaction(
            Guid id,
            TransactionType type,
            string billNo,
            Guid inventoryId,
            Guid reelId,
            Guid productId,
            decimal quantity,
            decimal quantityAfter,
            Guid? fromLocationId,
            Guid? toLocationId,
            Guid? fromWarehouseId,
            Guid? toWarehouseId,
            string sn,
            string batchNo,
            string craftVersion,
            InventoryStatus status,
            string remark = null) : base(id)
        {
            Type = type;
            BillNo = Check.NotNullOrWhiteSpace(billNo, nameof(billNo));
            InventoryId = inventoryId;
            ReelId = reelId;
            ProductId = productId;
            Quantity = quantity;
            QuantityAfter = quantityAfter;

            FromLocationId = fromLocationId;
            ToLocationId = toLocationId;
            FromWarehouseId = fromWarehouseId;
            ToWarehouseId = toWarehouseId;

            SN = sn;
            BatchNo = batchNo;
            CraftVersion = craftVersion;
            Status = status;
            Remark = remark;
        }
    }
}