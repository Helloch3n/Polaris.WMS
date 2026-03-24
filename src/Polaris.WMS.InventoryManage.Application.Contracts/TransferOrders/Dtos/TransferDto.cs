using Polaris.WMS.TransferOrders;
using Volo.Abp.Application.Dtos;

namespace Polaris.WMS.InventoryManage.Application.Contracts.TransferOrders.Dtos
{
    /// <summary>
    /// 调拨单列表输出。
    /// </summary>
    public class TransferListDto : AuditedEntityDto<Guid>
    {
        /// <summary>
        /// 调拨单号。
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 调拨单状态。
        /// </summary>
        public TransferOrderStatus Status { get; set; }

        /// <summary>
        /// 创建时间。
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 部门Id。
        /// </summary>
        public Guid? DepartmentId { get; set; }

        /// <summary>
        /// 部门编码。
        /// </summary>
        public string DepartmentCode { get; set; }

        /// <summary>
        /// 部门名称。
        /// </summary>
        public string DepartmentName { get; set; }

        /// <summary>
        /// 仓库Id。
        /// </summary>
        public Guid? WarehouseId { get; set; }

        /// <summary>
        /// 仓库编码。
        /// </summary>
        public string WarehouseCode { get; set; }

        /// <summary>
        /// 仓库名称。
        /// </summary>
        public string WarehouseName { get; set; }
    }

    /// <summary>
    /// 调拨单输出。
    /// </summary>
    public class TransferDto : AuditedEntityDto<Guid>
    {
        /// <summary>
        /// 调拨单号。
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 调拨单状态。
        /// </summary>
        public TransferOrderStatus Status { get; set; }

        /// <summary>
        /// 创建时间。
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 源仓库Id。
        /// </summary>
        public Guid? SourceWarehouseId { get; set; }

        /// <summary>
        /// 源仓库编码。
        /// </summary>
        public string SourceWarehouseCode { get; set; }

        /// <summary>
        /// 目标仓库Id。
        /// </summary>
        public Guid? TargetWarehouseId { get; set; }

        /// <summary>
        /// 目标仓库编码。
        /// </summary>
        public string TargetWarehouseCode { get; set; }

        /// <summary>
        /// 调拨明细。
        /// </summary>
        public List<TransferDetailDto> Details { get; set; } = new();
    }

    /// <summary>
    /// 调拨明细输出。
    /// </summary>
    public class TransferDetailDto
    {
        /// <summary>
        /// 明细Id。
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 盘具Id。
        /// </summary>
        public Guid ReelId { get; set; }

        /// <summary>
        /// 盘号。
        /// </summary>
        public string ReelCode { get; set; }

        /// <summary>
        /// 库存Id。
        /// </summary>
        public Guid InventoryId { get; set; }

        /// <summary>
        /// 物料Id。
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// 物料编码。
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// 物料名称。
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 调拨数量。
        /// </summary>
        public decimal Qty { get; set; }

        /// <summary>
        /// 源库位Id。
        /// </summary>
        public Guid SourceLocationId { get; set; }

        /// <summary>
        /// 源库位编码。
        /// </summary>
        public string SourceLocationCode { get; set; }

        /// <summary>
        /// 目标库位Id。
        /// </summary>
        public Guid TargetLocationId { get; set; }

        /// <summary>
        /// 目标库位编码。
        /// </summary>
        public string TargetLocationCode { get; set; }

        /// <summary>
        /// 源仓库Id。
        /// </summary>
        public Guid? SourceWarehouseId { get; set; }

        /// <summary>
        /// 源仓库编码。
        /// </summary>
        public string SourceWarehouseCode { get; set; }

        /// <summary>
        /// 目标仓库Id。
        /// </summary>
        public Guid? TargetWarehouseId { get; set; }

        /// <summary>
        /// 目标仓库编码。
        /// </summary>
        public string TargetWarehouseCode { get; set; }

        /// <summary>
        /// 是否完成。
        /// </summary>
        public bool IsCompleted { get; set; }

        /// <summary>
        /// 工艺版本
        /// </summary>
        public string CraftVersion { get; set; }
    }
}
