using Polaris.WMS.Isolation;
using Polaris.WMS.TransferOrders;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Polaris.WMS.InventoryManage.Domain.TransferOrders
{
    /// <summary>
    /// 调拨单聚合根。
    /// </summary>
    public class TransferOrder : FullAuditedAggregateRoot<Guid>, IMultiDepartment, IMultiWarehouse
    {
        // 必须实现接口要求的这两个属性，才能被 EF Core 拦截！
        public Guid DepartmentId { get; private set; }
        public Guid WarehouseId { get; private set; }
        /// <summary>
        /// 调拨单号。
        /// </summary>
        public string OrderNo { get; private set; }

        /// <summary>
        /// 调拨单状态。
        /// </summary>
        public TransferOrderStatus Status { get; private set; }

        /// <summary>
        /// 源仓库Id。
        /// </summary>
        public Guid SourceWarehouseId { get; private set; }

        /// <summary>
        /// 来源部门Id。
        /// </summary>
        public Guid SourceDepartmentId { get; private set; }

        /// <summary>
        /// 目标仓库Id。
        /// </summary>
        public Guid TargetWarehouseId { get; private set; }

        /// <summary>
        /// 调拨明细集合。
        /// </summary>
        private readonly List<TransferOrderDetail> _details = new();

        /// <summary>
        /// 只读调拨明细。
        /// </summary>
        public IReadOnlyCollection<TransferOrderDetail> Details => _details;

        protected TransferOrder()
        {
        }

        internal TransferOrder(
            Guid id,
            string orderNo,
            Guid sourceWarehouseId,
            Guid targetWarehouseId,
            Guid sourceDepartmentId) : base(id)
        {
            OrderNo = Check.NotNullOrWhiteSpace(orderNo, nameof(orderNo));
            if (sourceWarehouseId == Guid.Empty || targetWarehouseId == Guid.Empty)
            {
                throw new BusinessException("仓库信息不能为空");
            }

            SourceWarehouseId = sourceWarehouseId;
            TargetWarehouseId = targetWarehouseId;
            DepartmentId = sourceDepartmentId;
            WarehouseId = sourceWarehouseId;
            Status = TransferOrderStatus.Draft;
            SourceDepartmentId = sourceDepartmentId;
        }

        /// <summary>
        /// 创建调拨单聚合。
        /// </summary>
        /// <param name="id">调拨单Id。</param>
        /// <param name="orderNo">调拨单号。</param>
        /// <param name="sourceWarehouseId">源仓库Id。</param>
        /// <param name="targetWarehouseId">目标仓库Id。</param>
        /// <returns>调拨单聚合。</returns>
        public static TransferOrder Create(
            Guid id,
            string orderNo,
            Guid sourceWarehouseId,
            Guid targetWarehouseId,
            Guid sourceDepartmentId)
        {
            return new TransferOrder(id, orderNo, sourceWarehouseId, targetWarehouseId, sourceDepartmentId);
        }

        /// <summary>
        /// 更新单据仓库信息。
        /// </summary>
        public void UpdateWarehouses(Guid sourceWarehouseId, Guid targetWarehouseId)
        {
            EnsureDraftEditable();

            if (sourceWarehouseId == Guid.Empty || targetWarehouseId == Guid.Empty)
            {
                throw new BusinessException("仓库信息不能为空");
            }

            SourceWarehouseId = sourceWarehouseId;
            TargetWarehouseId = targetWarehouseId;
        }

        /// <summary>
        /// 更新调拨明细。
        /// </summary>
        public void UpdateDetail(
            Guid detailId,
            Guid reelId,
            Guid inventoryId,
            Guid productId,
            decimal qty,
            Guid sourceLocationId,
            Guid targetLocationId)
        {
            EnsureDraftEditable();

            var detail = _details.FirstOrDefault(x => x.Id == detailId);
            if (detail == null)
            {
                throw new BusinessException("未找到对应的调拨明细")
                    .WithData("明细Id", detailId);
            }

            detail.Update(reelId, inventoryId, productId, qty, sourceLocationId, targetLocationId);
        }

        /// <summary>
        /// 删除调拨明细。
        /// </summary>
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

        /// <summary>
        /// 校验草稿可编辑状态。
        /// </summary>
        private void EnsureDraftEditable()
        {
            if (Status != TransferOrderStatus.Draft)
            {
                throw new BusinessException("只有草稿状态的调拨单允许修改");
            }
        }


        /// <summary>
        /// 新增调拨明细。
        /// </summary>
        public TransferOrderDetail AddDetail(
            Guid detailId,
            Guid reelId,
            Guid inventoryId,
            Guid productId,
            decimal qty,
            Guid sourceLocationId,
            Guid targetLocationId)
        {
            if (Status is TransferOrderStatus.Completed or TransferOrderStatus.Cancelled)
            {
                throw new BusinessException("调拨单已完成或已取消，不能继续修改明细")
                    .WithData("Status", Status);
            }

            if (_details.Any(x => x.InventoryId == inventoryId && !x.IsCompleted))
            {
                throw new BusinessException("同一库存不能重复添加到未完成的调拨明细中")
                    .WithData("库存Id", inventoryId);
            }

            var detail = new TransferOrderDetail(
                detailId,
                Id,
                reelId,
                inventoryId,
                productId,
                qty,
                sourceLocationId,
                targetLocationId);

            _details.Add(detail);
            return detail;
        }

        /// <summary>
        /// 将指定明细标记为完成，并更新调拨单状态。
        /// </summary>
        /// <param name="detailId">明细Id。</param>
        public void MarkDetailAsCompleted(Guid detailId)
        {
            var detail = _details.FirstOrDefault(x => x.Id == detailId);
            if (detail == null)
            {
                throw new BusinessException("未找到对应的调拨明细")
                    .WithData("明细Id", detailId);
            }

            detail.MarkAsCompleted();

            if (_details.Count > 0 && _details.All(x => x.IsCompleted))
            {
                Completed();
            }
            else
            {
                Status = TransferOrderStatus.InProgress;
            }
        }

        public void Completed()
        {
            Status = TransferOrderStatus.Completed;
        }
    }
}
