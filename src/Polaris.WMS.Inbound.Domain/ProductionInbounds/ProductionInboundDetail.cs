using Polaris.WMS.ProductionInbounds;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Polaris.WMS.Inbound.Domain.ProductionInbounds
{
    /// <summary>
    /// 生产入库单明细表 (线缆行业专属物理载体设计)
    /// </summary>
    public class ProductionInboundDetail : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 关联的头表 ID
        /// </summary>
        public Guid ProductionInboundId { get; set; }

        /// <summary>
        /// 产品/物料 ID (半成品或成品)
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// 生产批次号 (品质追溯的核心依据)
        /// </summary>
        public string BatchNo { get; set; }

        /// <summary>
        /// 盘具ID
        /// </summary>
        public Guid ReelId { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal Qty { get; set; }

        /// <summary>
        /// 计量单位 (如：盘、卷、托)
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 净重 (kg) - 纯线缆的重量
        /// </summary>
        public decimal Weight { get; set; }


        /// <summary>
        /// 唯一标识
        /// </summary>
        public string SN { get; set; }

        /// <summary>
        /// 缠绕层级
        /// </summary>
        public int LayerIndex { get; set; }

        /// <summary>
        /// 关联的单号
        /// </summary>
        public string RelatedOrderNo { get; set; }

        /// <summary>
        /// 来源单号明细行
        /// </summary>
        public string RelatedOrderNoLineNo { get; set; }

        /// <summary>
        /// 工艺版本
        /// </summary>
        public string CraftVersion { get; set; }

        /// <summary>
        /// 明细行的作业状态 (追踪单盘入库进度)
        /// </summary>
        public ProductionInboundDetailStatus Status { get; set; }

        /// <summary>
        /// 实际入库库位 ID (扫码确认后记录的暂存区或正式货架的库位)
        /// </summary>
        public Guid ActualLocationId { get; set; }

        /// <summary>
        /// 供 EF Core 反射使用的无参构造函数
        /// </summary>
        protected ProductionInboundDetail() { }

        /// <summary>
        /// 最新业务构造函数（覆盖当前实体全部业务字段）
        /// </summary>
        internal ProductionInboundDetail(
            Guid id,
            Guid productionInboundId,
            Guid productId,
            string batchNo,
            Guid reelId,
            decimal qty,
            string unit,
            decimal weight,
            string sn,
            int layerIndex,
            string relatedOrderNo,
            string relatedOrderNoLineNo,
            Guid actualLocationId,
            string craftVersion,
            ProductionInboundDetailStatus status = ProductionInboundDetailStatus.Pending)
            : base(id)
        {
            ProductionInboundId = productionInboundId;
            ProductId = productId;
            BatchNo = Check.NotNullOrWhiteSpace(batchNo, nameof(batchNo));
            ReelId = reelId;
            Qty = qty;
            Unit = Check.NotNullOrWhiteSpace(unit, nameof(unit));

            Weight = weight;
            SN = sn;
            LayerIndex = layerIndex;
            RelatedOrderNo = relatedOrderNo;
            RelatedOrderNoLineNo = relatedOrderNoLineNo;
            ActualLocationId = actualLocationId;
            Status = status;
            CraftVersion = craftVersion;
        }


        /// <summary>
        /// 更新明细内容。
        /// </summary>
        internal void Update(
            Guid reelId,
            Guid productId,
            decimal qty,
            Guid actualLocationId,
            string craftVersion)
        {
            ReelId = reelId;
            ProductId = productId;
            Qty = qty > 0
                ? qty
                : throw new BusinessException("调拨数量必须大于 0")
                    .WithData("数量", qty);
            ActualLocationId = actualLocationId;
            CraftVersion = craftVersion;
        }

        internal void MarkAsCompleted()
        {
            Status = ProductionInboundDetailStatus.Completed;
        }
    }
}
