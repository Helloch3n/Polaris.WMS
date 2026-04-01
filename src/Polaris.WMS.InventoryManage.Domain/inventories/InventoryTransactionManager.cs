using Polaris.WMS.Inventories.Invnentory;
using Polaris.WMS.Inventories.Ivnentory;
using Polaris.WMS.InventoryManage.Domain.inventories.Args;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Polaris.WMS.InventoryManage.Domain.inventories
{
    /// <summary>
    /// 库存流水领域服务，负责库存流水创建规则。
    /// </summary>
    public class InventoryTransactionManager(
        IRepository<InventoryTransaction, Guid> transactionRepository
    ) : DomainService
    {
        /// <summary>
        /// 创建库存流水实体（仅构建实体，不执行持久化）。
        /// </summary>
        public async Task<InventoryTransaction> CreateAsync(CreateInventoryTranscationArgs args)
        {
            if (args.Quantity <= 0)
            {
                throw new BusinessException("InventoryTransaction:QuantityMustBePositive")
                    .WithData("Quantity", args.Quantity);
            }

            if (args.QuantityAfter < 0)
            {
                throw new BusinessException("InventoryTransaction:QuantityAfterCannotBeNegative")
                    .WithData("QuantityAfter", args.QuantityAfter);
            }

            var transaction = new InventoryTransaction(
                args.Id,
                args.Type,
                args.BillNo,
                args.InventoryId,
                args.ReelId,
                args.ProductId,
                args.Quantity,
                args.QuantityAfter,
                args.FromLocationId,
                args.ToLocationId,
                args.FromWarehouseId,
                args.ToWarehouseId,
                args.SN,
                args.BatchNo,
                args.CraftVersion,
                args.Status,
                args.Remark);
            await transactionRepository.InsertAsync(transaction);
            return transaction;
        }
    }
}