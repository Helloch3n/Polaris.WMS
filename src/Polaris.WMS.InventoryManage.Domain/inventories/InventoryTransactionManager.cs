using Polaris.WMS.Inventorys;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Polaris.WMS.InventoryManage.Domain.inventories
{
    /// <summary>
    /// 库存流水领域服务，负责库存流水创建规则。
    /// </summary>
    public class InventoryTransactionManager : DomainService
    {
        /// <summary>
        /// 创建库存流水实体（仅构建实体，不执行持久化）。
        /// </summary>
        public Task<InventoryTransaction> CreateAsync(
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
            string? remark = null)
        {
            if (quantity <= 0)
            {
                throw new BusinessException("InventoryTransaction:QuantityMustBePositive")
                    .WithData("Quantity", quantity);
            }

            if (quantityAfter < 0)
            {
                throw new BusinessException("InventoryTransaction:QuantityAfterCannotBeNegative")
                    .WithData("QuantityAfter", quantityAfter);
            }

            var transaction = new InventoryTransaction(
                id,
                type,
                billNo,
                inventoryId,
                reelId,
                productId,
                quantity,
                quantityAfter,
                fromLocationId,
                toLocationId,
                fromWarehouseId,
                toWarehouseId,
                sn,
                batchNo,
                craftVersion,
                status,
                remark);

            return Task.FromResult(transaction);
        }
    }
}
