using Polaris.WMS.MasterData.Domain.Integration.Inventory;
using Polaris.WMS.MasterData.Domain.warehouses;
using Polaris.WMS.MasterData.Domain.Zones;
using Polaris.WMS.MasterData.Warehouses;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Polaris.WMS.MasterData.Domain.Locations
{
    /// <summary>
    /// 库位领域服务，负责库位创建与状态维护等业务规则。
    /// </summary>
    public class LocationManager(
        ILocationRepository locationRepository,
        IZoneRepository zoneRepository,
        IRepository<Warehouse, Guid> warehouseRepository
        //IReelAllocationAdapter reelAllocationAdapter,
        //IInventoryAllocationAdapter inventoryAllocationAdapter
        )
        : DomainService
    {
        private IExternalReelProvider ExternalReelProvider => LazyServiceProvider.LazyGetRequiredService<IExternalReelProvider>();
        private IExternalInventoryProvider ExternalInventoryProvider => LazyServiceProvider.LazyGetRequiredService<IExternalInventoryProvider>();
        /// <summary>
        /// 根据库位装载情况刷新库位状态。
        /// </summary>
        /// <param name="locationId">库位Id。</param>
        public async Task RefreshStatusByLoadAsync(Guid locationId)
        {
            var location = await locationRepository.GetAsync(locationId);

            var reels = await ExternalReelProvider.GetReelInfosByLocationIdAsync(locationId);
            var reelCount = reels.Count;

            var reelIds = reels.Select(x => x.Id).ToList();
            var inventoryCount = 0;
            if (reelIds.Count > 0)
            {
                var inventorys = await ExternalInventoryProvider.GetInventoryByReels(reelIds);
                inventoryCount = inventorys.Count;
            }

            var newStatus = reelCount switch
            {
                0 => LocationStatus.Idle,
                _ when reelCount >= location.MaxReelCount => LocationStatus.Full,
                _ => LocationStatus.Partial
            };

            if (location.Status != newStatus)
            {
                location.ChangeStatus(newStatus);
                await locationRepository.UpdateAsync(location);
            }
        }

        /// <summary>
        /// 创建库位并执行归属及唯一性校验。
        /// </summary>
        public async Task<Location> CreateAsync(
            Guid warehouseId,
            Guid zoneId,
            string code,
            string aisle,
            string rack,
            string level,
            string bin,
            decimal maxWeight,
            decimal maxVolume,
            int maxReelCount,
            LocationType type = LocationType.Rack,
            bool allowMixedProducts = true,
            bool allowMixedBatches = true)
        {
            // 1. 校验仓库是否存在
            var warehouse = await warehouseRepository.GetAsync(warehouseId);
            if (warehouse == null)
            {
                throw new BusinessException("WMS:WarehouseNotFound").WithData("WarehouseId", warehouseId);
            }

            // 2. 校验库区是否存在且归属于指定仓库
            var zone = await zoneRepository.GetAsync(zoneId);
            if (zone == null)
            {
                throw new BusinessException("WMS:ZoneNotFound").WithData("ZoneId", zoneId);
            }

            if (zone.WarehouseId != warehouseId)
            {
                throw new BusinessException("WMS:ZoneNotBelongToWarehouse")
                    .WithData("ZoneId", zoneId)
                    .WithData("WarehouseId", warehouseId);
            }

            // 3. 校验库位编码在仓库内唯一
            var existing = await locationRepository.FindByCodeAsync(code);
            if (existing != null && existing.WarehouseId == warehouseId)
            {
                throw new BusinessException("WMS:LocationCodeAlreadyExists")
                    .WithData("Code", code)
                    .WithData("WarehouseId", warehouseId);
            }

            // 4. 构建并持久化库位实体
            var location = new Location(
                GuidGenerator.Create(),
                warehouseId,
                zoneId,
                code,
                aisle,
                rack,
                level,
                bin,
                maxWeight,
                maxVolume,
                maxReelCount,
                type,
                allowMixedProducts,
                allowMixedBatches);

            await locationRepository.InsertAsync(location);
            return location;
        }

        /// <summary>
        /// 按巷道/货架/层批量创建库位。
        /// </summary>
        public async Task<List<Location>> BatchCreateAsync(
            Guid warehouseId,
            Guid zoneId,
            string aislePrefix,
            int rackCount,
            int levelCount)
        {
            await warehouseRepository.GetAsync(warehouseId);

            var zone = await zoneRepository.GetAsync(zoneId);
            if (zone.WarehouseId != warehouseId)
            {
                throw new BusinessException("WMS:ZoneNotBelongToWarehouse")
                    .WithData("ZoneId", zoneId)
                    .WithData("WarehouseId", warehouseId);
            }

            var locations = new List<Location>();
            for (var rack = 1; rack <= rackCount; rack++)
            {
                for (var level = 1; level <= levelCount; level++)
                {
                    var code = $"{aislePrefix}-{rack:00}-{level:00}";
                    var location = new Location(
                        GuidGenerator.Create(),
                        warehouseId,
                        zoneId,
                        code,
                        aislePrefix,
                        rack.ToString("00"),
                        level.ToString("00"),
                        string.Empty,
                        0m,
                        0m,
                        1);
                    locations.Add(location);
                }
            }

            await locationRepository.InsertManyAsync(locations);
            return locations;
        }
    }
}