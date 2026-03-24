using Polaris.WMS.InventoryManage.Domain.Integration.Locations;
using Polaris.WMS.InventoryManage.Domain.Reels;
using Polaris.WMS.Inventorys;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Polaris.WMS.InventoryManage.Domain.inventories
{
    public class InventoryManager(
        IInventoryRepository inventoryRepository,
        IRepository<Reel, Guid> reelRepository,
        ReelManager reelManager,
        IRepository<InventoryTransaction, Guid> transactionRepository,
        InventoryTransactionManager inventoryTransactionManager
        //IExternalLocationAdapter externalLocationAdapter
        )
        : DomainService
    {
        private IExternalLocationAdapter ExternalLocationAdapter => LazyServiceProvider.LazyGetRequiredService<IExternalLocationAdapter>();
        public async Task<InventoryManage.Domain.inventories.Inventory> ProductionReceiveAsync(
            string orderNo,
            Guid reelId,
            Guid productId,
            decimal quantity,
            decimal weight,
            string batchNo,
            string? relatedOrderNo,
            string? relatedOrderLineNo,
            Guid locationId,
            string sn,
            string unit,
            string? craftVersion = null,
            int layerIndex = 0,
            InventoryStatus status = InventoryStatus.Good)
        {
            // SN全局唯一校验（如果SN不为空的话）
            var snExists = await inventoryRepository.IsSnExistsAsync(sn);
            if (snExists)
            {
                throw new BusinessException("WMS:DuplicateSN")
                    .WithData("sn", sn);
            }

            var reel = await reelRepository.GetAsync(reelId);

            var inventory = new InventoryManage.Domain.inventories.Inventory(
                GuidGenerator.Create(),
                reelId,
                productId,
                quantity,
                unit,
                weight,
                batchNo,
                relatedOrderNo,
                relatedOrderLineNo,
                Clock.Now,
                layerIndex,
                sn,
                craftVersion,
                status);

            //修改盘具为已占用
            reel.SetOccupied();
            //刷新库位状态
            //await locationManager.RefreshStatusByLoadAsync(locationId);
            await ExternalLocationAdapter.RefreshStatusByLoadAsync(locationId);

            await inventoryRepository.InsertAsync(inventory);
            await reelRepository.UpdateAsync(reel);

            //生成库存流水

            Guid? warehouseId = null;
            // var location = await locationRepository.GetAsync(locationId);
            var location = await ExternalLocationAdapter.GetLocationAsync(locationId);
            warehouseId = location.WarehouseId;

            var transaction = await inventoryTransactionManager.CreateAsync(
                id: GuidGenerator.Create(),
                type: TransactionType.Receipt,
                billNo: string.IsNullOrWhiteSpace(orderNo)
                    ? $"INV-ADD-{Clock.Now:yyyyMMddHHmmssfff}"
                    : orderNo,
                inventoryId: inventory.Id,
                reelId: inventory.ReelId,
                productId: inventory.ProductId,
                quantity: inventory.Quantity,
                quantityAfter: inventory.Quantity,
                fromLocationId: null,
                toLocationId: locationId,
                fromWarehouseId: null,
                toWarehouseId: warehouseId,
                sn: inventory.SN,
                batchNo: inventory.BatchNo,
                craftVersion: inventory.CraftVersion,
                status: inventory.Status,
                remark: "库存增加"
            );

            await transactionRepository.InsertAsync(transaction);

            return inventory;
        }

        /// <summary>
        /// 增加库存数量
        /// </summary>
        /// <param name="inventoryId">库存明细ID</param>
        /// <param name="qty">增加的数量</param>
        /// <param name="transType">业务类型 (必填，用于记账)</param>
        /// <param name="businessOrderNo">业务单号 (可选)</param>
        public async Task AddQuantityAsync(Guid inventoryId, decimal qty, TransactionType transType,
            string? businessOrderNo = null)
        {
            if (qty <= 0) throw new BusinessException("WMS:QtyMustBePositive").WithData("Qty", qty);

            // 1. 获取库存明细并校验状态
            var inventory = await inventoryRepository.GetAsync(inventoryId);
            if (inventory.Status != InventoryStatus.Good)
            {
                throw new BusinessException("库存状态不允许增加");
            }

            // 2. 改变库存数量
            inventory.AddQuantity(qty);
            await inventoryRepository.UpdateAsync(inventory);

            // 3. 强制生成“入库/增加”流水
            var reel = await reelRepository.GetAsync(inventory.ReelId);
            var locationId = reel.CurrentLocationId;

            Guid? warehouseId = null;
            if (locationId.HasValue)
            {
                //var location = await locationRepository.GetAsync(locationId.Value);
                var location = await ExternalLocationAdapter.GetLocationAsync(locationId.Value);
                warehouseId = location.WarehouseId;
            }

            var transaction = await inventoryTransactionManager.CreateAsync(
                id: GuidGenerator.Create(),
                type: transType,
                billNo: string.IsNullOrWhiteSpace(businessOrderNo)
                    ? $"INV-ADD-{Clock.Now:yyyyMMddHHmmssfff}"
                    : businessOrderNo,
                inventoryId: inventory.Id,
                reelId: inventory.ReelId,
                productId: inventory.ProductId,
                quantity: qty,
                quantityAfter: inventory.Quantity,
                fromLocationId: locationId,
                toLocationId: locationId,
                fromWarehouseId: warehouseId,
                toWarehouseId: warehouseId,
                sn: inventory.SN,
                batchNo: inventory.BatchNo,
                craftVersion: inventory.CraftVersion,
                status: inventory.Status,
                remark: "库存增加"
            );

            await transactionRepository.InsertAsync(transaction);
        }

        /// <summary>
        /// 扣减库存数量 (包含极其核心的“归零与虚拟盘销毁”逻辑)
        /// </summary>
        public async Task DeductQuantityAsync(Guid inventoryId, decimal qty, TransactionType transType,
            string? businessOrderNo = null)
        {
            if (qty <= 0) throw new BusinessException("WMS:QtyMustBePositive").WithData("Qty", qty);

            var inventory = await inventoryRepository.GetAsync(inventoryId);
            //if (inventory.Status == InventoryStatus.Locked)
            //{
            //    throw new BusinessException(WMSDomainErrorCodes.InventoryIsLocked);
            //}

            // 1. 跨聚合校验：防超扣
            if (inventory.Quantity < qty)
            {
                throw new BusinessException("WMS:InsufficientInventory")
                    .WithData("CurrentQty", inventory.Quantity)
                    .WithData("DeductQty", qty);
            }

            // 2. 改变库存数量
            inventory.DeductQuantity(qty);

            // 3. 强制生成“出库/扣减”流水
            var reel = await reelRepository.GetAsync(inventory.ReelId);
            var locationId = reel.CurrentLocationId;

            Guid? warehouseId = null;
            if (locationId.HasValue)
            {
                //var location = await locationRepository.GetAsync(locationId.Value);
                var location = await ExternalLocationAdapter.GetLocationAsync(locationId.Value);
                warehouseId = location.WarehouseId;
            }

            var transaction = await inventoryTransactionManager.CreateAsync(
                id: GuidGenerator.Create(),
                type: transType,
                billNo: string.IsNullOrWhiteSpace(businessOrderNo)
                    ? $"INV-DEDUCT-{Clock.Now:yyyyMMddHHmmssfff}"
                    : businessOrderNo,
                inventoryId: inventory.Id,
                reelId: inventory.ReelId,
                productId: inventory.ProductId,
                quantity: qty,
                quantityAfter: inventory.Quantity,
                fromLocationId: locationId,
                toLocationId: locationId,
                fromWarehouseId: warehouseId,
                toWarehouseId: warehouseId,
                sn: inventory.SN,
                batchNo: inventory.BatchNo,
                craftVersion: inventory.CraftVersion,
                status: inventory.Status,
                remark: "库存扣减"
            );

            await transactionRepository.InsertAsync(transaction);

            // 4. 终极逻辑：如果库存归零，处理盘具和库位状态
            if (inventory.Quantity == 0)
            {
                await HandleZeroInventoryAsync(inventory);
            }
            else
            {
                await inventoryRepository.UpdateAsync(inventory);
            }
        }

        /// <summary>
        /// 处理库存归零时的生命周期
        /// </summary>
        private async Task HandleZeroInventoryAsync(InventoryManage.Domain.inventories.Inventory inventory)
        {
            // A. 库存明细本体处理,软删除
            inventory.IsDeleted = true;
            await inventoryRepository.UpdateAsync(inventory);

            // B. 盘具生命周期处理
            if (inventory.ReelId != Guid.Empty)
            {
                var locationId = await reelManager.HandleReelAfterInventoryDepletedAsync(inventory.ReelId);

                // C. 库位释放处理 (如果盘具被删了，或者空了，检查所在的库位是否需要释放)
                if (locationId.HasValue)
                {
                    //await locationManager.RefreshStatusByLoadAsync(locationId.Value);
                    await ExternalLocationAdapter.RefreshStatusByLoadAsync(locationId.Value);
                }
            }
        }
    }
}