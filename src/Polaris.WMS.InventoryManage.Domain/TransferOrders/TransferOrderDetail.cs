using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace Polaris.WMS.InventoryManage.Domain.TransferOrders
{
    /// <summary>
    /// 调拨单明细实体。
    /// </summary>
    public class TransferOrderDetail : Entity<Guid>
    {
        /// <summary>
        /// 所属调拨单Id。
        /// </summary>
        public Guid TransferOrderId { get; private set; }

        /// <summary>
        /// 盘具Id。
        /// </summary>
        public Guid ContainerId { get; private set; }

        /// <summary>
        /// 库存Id。
        /// </summary>
        public Guid InventoryId { get; private set; }

        /// <summary>
        /// 物料Id。
        /// </summary>
        public Guid ProductId { get; private set; }

        /// <summary>
        /// 调拨数量。
        /// </summary>
        public decimal Qty { get; private set; }

        /// <summary>
        /// 源库位Id。
        /// </summary>
        public Guid SourceLocationId { get; private set; }

        /// <summary>
        /// 目标库位Id。
        /// </summary>
        public Guid TargetLocationId { get; private set; }

        /// <summary>
        /// 是否已完成。
        /// </summary>
        public bool IsCompleted { get; private set; }

        protected TransferOrderDetail()
        {
        }

        internal TransferOrderDetail(
            Guid id,
            Guid transferOrderId,
            Guid containerId,
            Guid inventoryId,
            Guid productId,
            decimal qty,
            Guid sourceLocationId,
            Guid targetLocationId)
            : base(id)
        {
            TransferOrderId = transferOrderId;
            ContainerId = containerId;
            InventoryId = inventoryId;
            ProductId = productId;
            Qty = qty > 0
                ? qty
                : throw new BusinessException("调拨数量必须大于 0")
                    .WithData("数量", qty);
            SourceLocationId = sourceLocationId;
            TargetLocationId = targetLocationId;
            IsCompleted = false;
        }

        /// <summary>
        /// 标记明细完成。
        /// </summary>
        internal void MarkAsCompleted()
        {
            IsCompleted = true;
        }

        /// <summary>
        /// 更新明细内容。
        /// </summary>
        internal void Update(
            Guid containerId,
            Guid inventoryId,
            Guid productId,
            decimal qty,
            Guid sourceLocationId,
            Guid targetLocationId)
        {
            ContainerId = containerId;
            InventoryId = inventoryId;
            ProductId = productId;
            Qty = qty > 0
                ? qty
                : throw new BusinessException("调拨数量必须大于 0")
                    .WithData("数量", qty);
            SourceLocationId = sourceLocationId;
            TargetLocationId = targetLocationId;
        }
    }
}
