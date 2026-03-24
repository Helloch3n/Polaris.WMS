using Polaris.WMS.Isolation;
using Polaris.WMS.ProductionInbounds;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Polaris.WMS.Inbound.Domain.ProductionInbounds
{
    public class ProductionInbound : FullAuditedAggregateRoot<Guid>, IMultiDepartment, IMultiWarehouse
    {
        /// <summary>
        /// WMS 生产入库单号
        /// </summary>
        public string OrderNo { get; private set; }

        /// <summary>
        /// 来源单号 (关联外部的生产任务单号、排程单号或 LIMS 检验单号)
        /// </summary>
        public string SourceOrderNo { get; private set; }

        /// <summary>
        /// 来源部门 ID (产生这批物料的制造车间/班组)
        /// </summary>
        public Guid SourceDepartmentId { get; private set; }

        /// <summary>
        /// 目标仓库 ID (入库后物料存放的仓库)
        /// </summary>
        public Guid TargetWarehouseId { get; private set; }

        /// <summary>
        /// 归属仓库 ID (数据隔离墙字段：当前单据的管理仓库)
        /// </summary>
        public Guid WarehouseId { get; private set; }

        /// <summary>
        /// 归属部门 ID (数据隔离墙字段：当前单据的管理部门)
        /// </summary>
        public Guid DepartmentId { get; private set; }

        /// <summary>
        /// 当前入库单的总体状态
        /// </summary>
        public ProductionInboundStatus Status { get; private set; }

        /// <summary>
        /// 入库单的类型 (成品入库、半成品入库、工序品入库)
        /// </summary>
        public ProductionInboundType InboundType { get; private set; }

        /// <summary>
        /// 只读入库明细集合 (外部只能读取，不能直接 Add/Remove)
        /// </summary>
        private readonly List<ProductionInboundDetail> _details = new();
        public IReadOnlyCollection<ProductionInboundDetail> Details => _details;


        /// <summary>
        /// 供 EF Core 反射使用的无参构造函数
        /// </summary>
        protected ProductionInbound() { }

        /// <summary>
        /// 业务构造函数
        /// </summary>
        public ProductionInbound(
            Guid id,
            string orderNo,
            string sourceOrderNo,
            Guid sourceDepartmentId,
            Guid targetWarehouseId,
            ProductionInboundType inboundType
            )
            : base(id)
        {
            OrderNo = orderNo;
            SourceOrderNo = sourceOrderNo;
            SourceDepartmentId = sourceDepartmentId;
            TargetWarehouseId = targetWarehouseId;
            DepartmentId = sourceDepartmentId;
            WarehouseId = targetWarehouseId;
            Status = ProductionInboundStatus.Draft; // 初始状态为草稿
            InboundType = inboundType;
        }

        /// <summary>
        /// 静态工厂方法：外部创建生产入库单头表的唯一合法途径
        /// </summary>
        public static ProductionInbound Create(
         Guid id,
         string orderNo,
         string sourceOrderNo,
         Guid sourceDepartmentId,
         Guid targetWarehouseId,
         ProductionInboundType inboundType)
        {
            // 校验单号是否为空
            if (string.IsNullOrWhiteSpace(orderNo)) throw new ArgumentException("单号不能为空");

            return new ProductionInbound(id, orderNo, sourceOrderNo, sourceDepartmentId, targetWarehouseId, inboundType);
        }

        /// <summary>
        /// 领域方法：向入库单中添加明细行
        /// </summary>
        public void AddDetail(
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
            string craftVersion)
        {
            var detail = new ProductionInboundDetail(
            id,
            productionInboundId,
            productId,
            batchNo,
            reelId,
            qty,
            unit,
            weight,
            sn,
            layerIndex,
            relatedOrderNo,
            relatedOrderNoLineNo,
            actualLocationId,
            craftVersion);
            _details.Add(detail);
        }

        /// <summary>
        /// 将指定明细标记为完成，并更新入库单状态。
        /// </summary>
        /// <param name="detailId">明细Id。</param>
        public void MarkDetailAsCompleted(Guid detailId)
        {
            var detail = _details.FirstOrDefault(x => x.Id == detailId);
            if (detail == null)
            {
                throw new BusinessException("未找到对应的入库明细")
                    .WithData("明细Id", detailId);
            }

            detail.MarkAsCompleted();

            if (_details.Count > 0 && _details.All(x => x.Status == ProductionInboundDetailStatus.Completed))
            {
                Completed();
            }
            else
            {
                Status = ProductionInboundStatus.InProgress;
            }
        }

        public void Completed()
        {
            Status = ProductionInboundStatus.Completed;
        }

        /// <summary>
        /// 校验草稿可编辑状态。
        /// </summary>
        private void EnsureDraftEditable()
        {
            if (Status != ProductionInboundStatus.Draft)
            {
                throw new BusinessException("只有草稿状态的调拨单允许修改");
            }
        }

        /// <summary>
        /// 更新明细
        /// </summary>
        /// <param name="detailId"></param>
        /// <param name="reelId"></param>
        /// <param name="inventoryId"></param>
        /// <param name="productId"></param>
        /// <param name="qty"></param>
        /// <param name="sourceLocationId"></param>
        /// <param name="targetLocationId"></param>
        public void UpdateDetail(
           Guid detailId,
           Guid reelId,
            Guid productId,
            decimal qty,
            Guid actualLocationId,
            string craftVersion)
        {
            EnsureDraftEditable();

            var detail = _details.FirstOrDefault(x => x.Id == detailId);
            if (detail == null)
            {
                throw new BusinessException("未找到对应的入库单明细")
                    .WithData("明细Id", detailId);
            }

            detail.Update(reelId, productId, qty, actualLocationId, craftVersion);
        }

        /// <summary>
        /// 删除明细
        /// </summary>
        /// <param name="detailId"></param>
        public void RemoveDetail(Guid detailId)
        {
            EnsureDraftEditable();

            var detail = _details.FirstOrDefault(x => x.Id == detailId);
            if (detail == null)
            {
                return;
            }

            _details.Remove(detail);
        }
    }
}
