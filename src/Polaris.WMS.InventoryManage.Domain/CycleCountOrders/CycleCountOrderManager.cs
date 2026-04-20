using Polaris.WMS.Inventories.Transaction;
using Polaris.WMS.InventoryManage.Domain.Containers;
using Polaris.WMS.InventoryManage.Domain.inventories;
using Polaris.WMS.InventoryManage.Domain.inventories.Args;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Polaris.WMS.InventoryManage.Domain.CycleCountOrders;

public class CycleCountOrderManager(
    IRepository<Container, Guid> containerRepository,
    IRepository<Inventory, Guid> inventoryRepository,
    InventoryTransactionManager inventoryTransactionManager)
    : DomainService
{
    private Domain.Integration.Locations.IExternalLocationProvider ExternalLocationProvider =>
        LazyServiceProvider.LazyGetRequiredService<Domain.Integration.Locations.IExternalLocationProvider>();

    public async Task SubmitCountResultAsync(
        CycleCountOrder order,
        string containerCode,
        Guid productId,
        decimal countedQty)
    {
        var detail = order.SubmitCountResult(containerCode, productId, countedQty);

        if (detail.DifferenceQty != 0)
        {
            await AdjustInventoryByDifferenceAsync(order, detail);
            detail.MarkInventoryAdjusted();
        }

        if (order.CanComplete())
        {
            order.Complete();
        }
    }

    private async Task AdjustInventoryByDifferenceAsync(
        CycleCountOrder order,
        CycleCountOrderDetail detail)
    {
        var container = await containerRepository.FirstOrDefaultAsync(x => x.ContainerCode == detail.ContainerCode);
        if (container == null)
        {
            throw new BusinessException("盘点调整失败：托盘不存在")
                .WithData("ContainerCode", detail.ContainerCode);
        }

        var invQuery = await inventoryRepository.GetQueryableAsync();
        var inventories = await AsyncExecuter.ToListAsync(
            invQuery.Where(x =>
                    x.ContainerId == container.Id &&
                    x.ProductId == detail.ProductId &&
                    !x.IsDeleted)
                .OrderByDescending(x => x.Sequence)
                .ThenByDescending(x => x.CreationTime));

        if (!inventories.Any())
        {
            throw new BusinessException("盘点调整失败：未找到库存")
                .WithData("ContainerCode", detail.ContainerCode)
                .WithData("ProductId", detail.ProductId);
        }

        Guid? locationId = detail.LocationId != Guid.Empty
            ? detail.LocationId
            : container.CurrentLocationId;

        Guid? warehouseId = null;
        if (locationId.HasValue)
        {
            var location = await ExternalLocationProvider.GetLocationAsync(locationId.Value);
            warehouseId = location?.WarehouseId;
        }

        if (detail.DifferenceQty > 0)
        {
            var targetInventory = inventories.First();
            targetInventory.AddQuantity(detail.DifferenceQty);
            await inventoryRepository.UpdateAsync(targetInventory);

            await inventoryTransactionManager.CreateAsync(new CreateInventoryTranscationArgs
            {
                Id = GuidGenerator.Create(),
                Type = TransactionType.Adjust,
                BillNo = order.OrderNo,
                InventoryId = targetInventory.Id,
                ContainerId = targetInventory.ContainerId,
                ProductId = targetInventory.ProductId,
                Quantity = detail.DifferenceQty,
                QuantityAfter = targetInventory.Quantity,
                FromLocationId = locationId,
                ToLocationId = locationId,
                FromWarehouseId = warehouseId,
                ToWarehouseId = warehouseId,
                SN = targetInventory.SN,
                BatchNo = targetInventory.BatchNo,
                CraftVersion = targetInventory.CraftVersion,
                Status = targetInventory.Status,
                Remark = "盘点盈余调整"
            });

            return;
        }

        var needDeduct = Math.Abs(detail.DifferenceQty);
        foreach (var inventory in inventories)
        {
            if (needDeduct <= 0)
            {
                break;
            }

            if (inventory.Quantity <= 0)
            {
                continue;
            }

            var deductQty = Math.Min(inventory.Quantity, needDeduct);
            inventory.Reserve(deductQty);
            inventory.DeductQuantity(deductQty);
            await inventoryRepository.UpdateAsync(inventory);

            await inventoryTransactionManager.CreateAsync(new CreateInventoryTranscationArgs
            {
                Id = GuidGenerator.Create(),
                Type = TransactionType.Adjust,
                BillNo = order.OrderNo,
                InventoryId = inventory.Id,
                ContainerId = inventory.ContainerId,
                ProductId = inventory.ProductId,
                Quantity = deductQty,
                QuantityAfter = inventory.Quantity,
                FromLocationId = locationId,
                ToLocationId = locationId,
                FromWarehouseId = warehouseId,
                ToWarehouseId = warehouseId,
                SN = inventory.SN,
                BatchNo = inventory.BatchNo,
                CraftVersion = inventory.CraftVersion,
                Status = inventory.Status,
                Remark = "盘点亏损调整"
            });

            needDeduct -= deductQty;
        }

        if (needDeduct > 0)
        {
            throw new BusinessException("盘点调整失败：库存不足，无法完成差异扣减")
                .WithData("RemainingDeductQty", needDeduct);
        }
    }
}
