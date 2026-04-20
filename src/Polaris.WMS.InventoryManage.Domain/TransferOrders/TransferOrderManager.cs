using Polaris.WMS.BillNumbers;
using Polaris.WMS.InventoryManage.Domain.Containers;
using Polaris.WMS.TransferOrders;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Polaris.WMS.InventoryManage.Domain.TransferOrders
{
    /// <summary>
    /// 调拨单领域服务。
    /// </summary>
    public class TransferOrderManager(
        IRepository<TransferOrder, Guid> transferOrderRepository,
        IRepository<Container, Guid> containerRepository,
        IBillNumberGenerator billNumberGenerator,
        ContainerManager containerManager)
        : DomainService
    {
        /// <summary>
        /// 创建调拨单聚合（仅构建实体，不持久化）。
        /// </summary>
        /// <returns>调拨单聚合。</returns>
        public async Task<TransferOrder> CreateAsync(Guid id, Guid sourceWarehouseId, Guid targetWarehouseId,Guid sourceDepartmentId)
        {
            if (sourceWarehouseId == Guid.Empty || targetWarehouseId == Guid.Empty)
            {
                throw new BusinessException("源仓库和目标仓库不能为空");
            }

            var orderNo = await billNumberGenerator.GetNextNumberAsync("TRF");
            var normalizedOrderNo = Check.NotNullOrWhiteSpace(orderNo, nameof(orderNo));

            var exists = await transferOrderRepository.AnyAsync(x => x.OrderNo == normalizedOrderNo);
            if (exists)
            {
                throw new BusinessException("调拨单号已存在")
                    .WithData("调拨单号", normalizedOrderNo);
            }

            return TransferOrder.Create(id, normalizedOrderNo, sourceWarehouseId, targetWarehouseId, sourceDepartmentId);
        }

        /// <summary>
        /// 审核并执行调拨单。
        /// </summary>
        /// <param name="orderId">调拨单Id。</param>
        public async Task ApproveAndExecuteAsync(Guid orderId)
        {
            // 1. 加载调拨单并校验状态
            var query = await transferOrderRepository.WithDetailsAsync(x => x.Details);
            var order = await AsyncExecuter.FirstOrDefaultAsync(query.Where(x => x.Id == orderId));
            if (order == null)
            {
                throw new BusinessException("调拨单不存在")
                    .WithData("调拨单Id", orderId);
            }

            if (order.Status != TransferOrderStatus.Draft)
            {
                throw new BusinessException("只有草稿状态才能审核执行！");
            }

            // 2. 先标记所有明细完成，驱动聚合状态流转
            foreach (var detail in order.Details)
            {
                order.MarkDetailAsCompleted(detail.Id);
            }

            // 3. 按 ContainerId + TargetLocationId 去重，避免同一盘具重复移库
            var uniqueTransfers = order.Details
                .GroupBy(x => new { x.ContainerId, x.TargetLocationId })
                .Select(x => x.First())
                .ToList();

            // 4. 执行物理移库（由盘具领域服务负责位置与流水）
            foreach (var detail in uniqueTransfers)
            {
                await containerManager.TransferLocationAsync(
                    detail.ContainerId,
                    detail.TargetLocationId,
                    order.OrderNo
                );
            }

            // 5. 持久化调拨单最新状态
            await transferOrderRepository.UpdateAsync(order);
        }

        /// <summary>
        /// 删除调拨单（仅允许草稿状态）。
        /// </summary>
        /// <param name="transferOrderId">调拨单Id。</param>
        public async Task DeleteAsync(Guid transferOrderId)
        {
            // 1. 加载调拨单及明细
            var query = await transferOrderRepository.WithDetailsAsync(x => x.Details);
            var order = await AsyncExecuter.FirstOrDefaultAsync(query.Where(x => x.Id == transferOrderId));
            if (order == null)
            {
                throw new BusinessException("调拨单不存在")
                    .WithData("调拨单Id", transferOrderId);
            }

            // 2. 仅允许删除草稿单
            if (order.Status != TransferOrderStatus.Draft)
            {
                throw new BusinessException("只有草稿状态的调拨单允许删除");
            }

            // 3. 解锁该单据明细关联的盘具
            var containerIds = order.Details.Select(x => x.ContainerId).Distinct().ToList();
            if (containerIds.Count > 0)
            {
                var reels = await containerRepository.GetListAsync(x => containerIds.Contains(x.Id));
                foreach (var container in reels)
                {
                    if (container.IsLocked)
                    {
                        container.UnLock();
                        await containerRepository.UpdateAsync(container);
                    }
                }
            }

            // 4. 删除调拨单（明细通过级联删除）
            await transferOrderRepository.DeleteAsync(order, autoSave: true);
        }
    }
}
