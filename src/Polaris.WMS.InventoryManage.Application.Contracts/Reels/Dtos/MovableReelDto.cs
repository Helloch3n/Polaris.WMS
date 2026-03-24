namespace Polaris.WMS.InventoryManage.Application.Contracts.Reels.Dtos
{
    /// <summary>
    /// 可搬运的盘具 DTO (前端弹窗选盘使用，以盘具为聚合根)
    /// </summary>
    public class MovableReelDto
    {
        /// <summary>
        /// 盘具 ID (前端勾选的核心依据)
        /// </summary>
        public Guid ReelId { get; set; }

        /// <summary>
        /// 盘具条码/编号
        /// </summary>
        public string ReelNo { get; set; }

        /// <summary>
        /// 盘具类型 (如：周转盘、成品盘。方便前端做特殊图标展示)
        /// </summary>
        public string ReelType { get; set; }

        /// <summary>
        /// 当前所在的库位 ID
        /// </summary>
        public Guid CurrentLocationId { get; set; }

        /// <summary>
        /// 当前所在的库位编码 (如：A01-01-05)
        /// </summary>
        public string CurrentLocationCode { get; set; }

        /// <summary>
        /// 该盘具下挂载的所有库存明细 (树形结构的子节点)
        /// </summary>
        public List<InventoryBriefDto> Inventories { get; set; } = new List<InventoryBriefDto>();
    }

    /// <summary>
    /// 库存明细简要信息 DTO (作为 MovableReelDto 的子项展示)
    /// </summary>
    public class InventoryBriefDto
    {
        /// <summary>
        /// 库存 ID (未来生成调拨明细行时需要记录)
        /// </summary>
        public Guid InventoryId { get; set; }

        /// <summary>
        /// 物料/产品 ID
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// 物料编码 (如：CBL-120)
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// 物料名称/规格 (如：单芯铜导线 120mm²)
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 批次号 (线缆行业极其重要)
        /// </summary>
        public string BatchNo { get; set; }

        /// <summary>
        /// 实际数量 (长度/重量等)
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// 计量单位 (如：M, KG)
        /// </summary>
        public string Uom { get; set; }
    }
}
