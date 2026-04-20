using Polaris.WMS.Inventories.Invnentory;
using Polaris.WMS.Inventories.Invnentory.Events;
using Polaris.WMS.Inventories.Transaction;
using Polaris.WMS.InventoryManage.Domain.Integration.Locations;
using Polaris.WMS.InventoryManage.Domain.inventories.Args;
using Polaris.WMS.InventoryManage.Domain.Containers;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.EventBus.Local;

namespace Polaris.WMS.InventoryManage.Domain.inventories
{
    public class InventoryManager(
        IInventoryRepository inventoryRepository,
        IRepository<Container, Guid> containerRepository,
        ContainerManager containerManager,
        IRepository<InventoryTransaction, Guid> transactionRepository,
        InventoryTransactionManager inventoryTransactionManager,
        IDistributedEventBus distributedEventBus,
        ILocalEventBus localEventBus
        //IExternalLocationAdapter externalLocationAdapter
    )
        : DomainService
    {
        private IExternalLocationProvider ExternalLocationProvider =>
            LazyServiceProvider.LazyGetRequiredService<IExternalLocationProvider>();

        public async Task ReceiveByContainerAsync(ReceiveContainerArgs args)
        {
            //获取盘具信息
            var container = await containerRepository.GetAsync(args.ContainerId);
            //获取库位信息
            var location = await ExternalLocationProvider.GetLocationAsync(args.LocationId);
            //修改盘具为已占用
            container.SetOccupied();
            //刷新库位状态
            await ExternalLocationProvider.RefreshStatusByLoadAsync(args.LocationId);
            //更新container
            await containerRepository.UpdateAsync(container);

            //循环 args.Items 创建 Inventory 实体
            foreach (var item in args.Items)
            {
                //校验SN唯一性
                var snExists = await inventoryRepository.IsSnExistsAsync(item.SN);
                if (snExists)
                {
                    throw new BusinessException("WMS:DuplicateSN")
                        .WithData("sn", item.SN);
                }

                //插入inventory
                var inventory = Inventory.CreateHoldInventory(GuidGenerator.Create(),
                    args.ContainerId,
                    item.ProductId,
                    item.Qty,
                    item.Unit,
                    item.Weight,
                    item.BatchNo,
                    item.RelatedOrderNo,
                    item.RelatedOrderLineNo,
                    Clock.Now,
                    item.LayerIndex,
                    item.SN,
                    item.CraftVersion,
                    item.Status);

                await inventoryRepository.InsertAsync(inventory);

                //发入库待检事件
                await localEventBus.PublishAsync(new HoldInventoryCreatedEto()
                {
                    ContainerId = args.ContainerId,
                    ContainerCode = container.ContainerCode,
                    CurrentLocationId = args.LocationId,
                    CurrentLocationCode = location.Code
                });

                //生成库存流水
                Guid? warehouseId = null;
                //var location = await ExternalLocationProvider.GetLocationAsync(args.LocationId);
                warehouseId = location.WarehouseId;

                var input = new CreateInventoryTranscationArgs()
                {
                    Id = GuidGenerator.Create(),
                    Type = TransactionType.Receipt,
                    BillNo = string.IsNullOrWhiteSpace(args.OrderNo)
                        ? $"INV-ADD-{Clock.Now:yyyyMMddHHmmssfff}"
                        : args.OrderNo,
                    InventoryId = inventory.Id,
                    ContainerId = inventory.ContainerId,
                    ProductId = inventory.ProductId,
                    Quantity = inventory.Quantity,
                    QuantityAfter = inventory.Quantity,
                    FromLocationId = null,
                    ToLocationId = args.LocationId,
                    FromWarehouseId = null,
                    ToWarehouseId = warehouseId,
                    SN = inventory.SN,
                    BatchNo = inventory.BatchNo,
                    CraftVersion = inventory.CraftVersion,
                    Status = inventory.Status,
                    Remark = "入库"
                };

                await inventoryTransactionManager.CreateAsync(input);
            }
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
            var container = await containerRepository.GetAsync(inventory.ContainerId);
            var locationId = container.CurrentLocationId;

            Guid? warehouseId = null;
            if (locationId.HasValue)
            {
                //var location = await locationRepository.GetAsync(locationId.Value);
                var location = await ExternalLocationProvider.GetLocationAsync(locationId.Value);
                warehouseId = location.WarehouseId;
            }

            var createArgs = new CreateInventoryTranscationArgs()
            {
                Id = GuidGenerator.Create(),
                Type = transType,
                BillNo = string.IsNullOrWhiteSpace(businessOrderNo)
                    ? $"INV-ADD-{Clock.Now:yyyyMMddHHmmssfff}"
                    : businessOrderNo,
                InventoryId = inventory.Id,
                ContainerId = inventory.ContainerId,
                ProductId = inventory.ProductId,
                Quantity = qty,
                QuantityAfter = inventory.Quantity,
                FromLocationId = locationId,
                ToLocationId = locationId,
                FromWarehouseId = warehouseId,
                ToWarehouseId = warehouseId,
                SN = inventory.SN,
                BatchNo = inventory.BatchNo,
                CraftVersion = inventory.CraftVersion,
                Status = inventory.Status,
                Remark = "库存增加"
            };

            await inventoryTransactionManager.CreateAsync(createArgs);
        }

        /// <summary>
        /// 扣减库存数量 (包含极其核心的“归零与虚拟盘销毁”逻辑)
        /// </summary>
        public async Task DeductQuantityAsync(Guid inventoryId, decimal qty, TransactionType transType,
            string? businessOrderNo = null)
        {
            if (qty <= 0) throw new BusinessException("WMS:QtyMustBePositive").WithData("Qty", qty);

            var inventory = await inventoryRepository.GetAsync(x => x.Id == inventoryId);
            if (inventory == null)
            {
                throw new UserFriendlyException($"未找到Id为{inventoryId}的库存");
            }

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
            var container = await containerRepository.GetAsync(inventory.ContainerId);
            var locationId = container.CurrentLocationId;

            Guid? warehouseId = null;
            if (locationId.HasValue)
            {
                //var location = await locationRepository.GetAsync(locationId.Value);
                var location = await ExternalLocationProvider.GetLocationAsync(locationId.Value);
                if (location == null)
                {
                    throw new UserFriendlyException($"未找到Id为{locationId.Value}的库位");
                }

                warehouseId = location.WarehouseId;
            }

            var deductArgs = new CreateInventoryTranscationArgs()
            {
                Id = GuidGenerator.Create(),
                Type = transType,
                BillNo = string.IsNullOrWhiteSpace(businessOrderNo)
                    ? $"INV-DEDUCT-{Clock.Now:yyyyMMddHHmmssfff}"
                    : businessOrderNo,
                InventoryId = inventory.Id,
                ContainerId = inventory.ContainerId,
                ProductId = inventory.ProductId,
                Quantity = qty,
                QuantityAfter = inventory.Quantity,
                FromLocationId = locationId,
                ToLocationId = locationId,
                FromWarehouseId = warehouseId,
                ToWarehouseId = warehouseId,
                SN = inventory.SN,
                BatchNo = inventory.BatchNo,
                CraftVersion = inventory.CraftVersion,
                Status = inventory.Status,
                Remark = "库存扣减"
            };

            await inventoryTransactionManager.CreateAsync(deductArgs);

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
            if (inventory.ContainerId != Guid.Empty)
            {
                var locationId = await containerManager.HandleReelAfterInventoryDepletedAsync(inventory.ContainerId);

                // C. 库位释放处理 (如果盘具被删了，或者空了，检查所在的库位是否需要释放)
                if (locationId.HasValue)
                {
                    //await locationManager.RefreshStatusByLoadAsync(locationId.Value);
                    await ExternalLocationProvider.RefreshStatusByLoadAsync(locationId.Value);
                }
            }
        }
    }
}