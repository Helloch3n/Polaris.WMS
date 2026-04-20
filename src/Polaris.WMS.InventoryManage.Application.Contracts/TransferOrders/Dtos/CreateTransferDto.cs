namespace Polaris.WMS.InventoryManage.Application.Contracts.TransferOrders.Dtos
{
    /// <summary>
    /// 创建调拨单输入。
    /// </summary>
    public class CreateTransferDto
    {
        /// <summary>
        /// 源仓库Id。
        /// </summary>
        public Guid SourceWarehouseId { get; set; }

        /// <summary>
        /// 来源部门Id。
        /// </summary>
        public Guid SourceDepartmentId { get; set; }

        /// <summary>
        /// 目标仓库Id。
        /// </summary>
        public Guid TargetWarehouseId { get; set; }

        /// <summary>
        /// 调拨明细。
        /// </summary>
        public List<CreateTransferDetailDto> Details { get; set; } = new();
    }

    /// <summary>
    /// 创建调拨明细输入。
    /// </summary>
    public class CreateTransferDetailDto
    {
        /// <summary>
        /// 盘具Id。
        /// </summary>
        public Guid ContainerId { get; set; }

        /// <summary>
        /// 库存Id。
        /// </summary>
        public Guid InventoryId { get; set; }

        /// <summary>
        /// 物料Id。
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// 调拨数量。
        /// </summary>
        public decimal Qty { get; set; }

        /// <summary>
        /// 源库位Id。
        /// </summary>
        public Guid SourceLocationId { get; set; }

        /// <summary>
        /// 目标库位Id。
        /// </summary>
        public Guid TargetLocationId { get; set; }
    }
}
