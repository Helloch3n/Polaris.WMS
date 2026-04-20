using System.Runtime.InteropServices;
using Polaris.WMS.Inventories.Invnentory;
using Polaris.WMS.Inventories.Ivnentory;

namespace Polaris.WMS.InventoryManage.Application.Contracts.Inventories.Dtos
{
    /// <summary>
    /// 库存明细 DTO。
    /// </summary>
    public class InventoryDto
    {
        /// <summary>
        /// 主键。
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 盘具Id。
        /// </summary>
        public Guid ContainerId { get; set; }

        /// <summary>
        /// 物料Id。
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// 库存总量。
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// 可用量。
        /// </summary>
        public decimal AvailableQuantity { get; set; }

        /// <summary>
        /// 锁定量。
        /// </summary>
        public decimal LockedQuantity { get; set; }

        /// <summary>
        /// 单位。
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 重量。
        /// </summary>
        public decimal Weight { get; set; }

        /// <summary>
        /// 批次号。
        /// </summary>
        public string BatchNo { get; set; }

        /// <summary>
        /// 所属单据。
        /// </summary>
        public string? RelatedOrderNo { get; set; }

        /// <summary>
        /// 所属单据明细行。
        /// </summary>
        public string? RelatedOrderLineNo { get; set; }

        /// <summary>
        /// 序列号。
        /// </summary>
        public string SN { get; set; }

        /// <summary>
        /// 库存类型。
        /// </summary>
        public InventoryType Type { get; set; }

        /// <summary>
        /// 库存状态。
        /// </summary>
        public InventoryStatus Status { get; set; }

        /// <summary>
        /// 工艺版本。
        /// </summary>
        public string? CraftVersion { get; set; }

        /// <summary>
        /// FIFO 时间。
        /// </summary>
        public DateTime FifoDate { get; set; }

        /// <summary>
        /// 层号。
        /// </summary>
        public int LayerIndex { get; set; }

        public int Sequence { get; set; }

        /// <summary>
        /// 盘号。
        /// </summary>
        public string ContainerCode { get; set; }

        /// <summary>
        /// 库位编码。
        /// </summary>
        public string LocationCode { get; set; }

        /// <summary>
        /// 物料名称。
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 物料编码。
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// 仓库名称。
        /// </summary>
        public string WarehouseName { get; set; }

        /// <summary>
        /// 仓库编码。
        /// </summary>
        public string WarehouseCode { get; set; }

        /// <summary>
        /// 库区编码。
        /// </summary>
        public string ZoneCode { get; set; }

        /// <summary>
        /// 库区名称。
        /// </summary>
        public string ZoneName { get; set; }
    }
}