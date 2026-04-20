using Microsoft.AspNetCore.Authorization;
using Polaris.WMS.InventoryManage.Application.Contracts.Inventories;
using Polaris.WMS.InventoryManage.Application.Contracts.Inventories.Dtos;
using Polaris.WMS.InventoryManage.Domain.inventories;
using Polaris.WMS.InventoryManage.Domain.Containers;
using Polaris.WMS.MasterData.Application.Contracts.Integration.Locations;
using Polaris.WMS.MasterData.Application.Contracts.Integration.Products;
using Polaris.WMS.MasterData.Application.Contracts.Integration.Warehouses;
using Polaris.WMS.MasterData.Application.Contracts.Integration.Zones;
using Polaris.WMS.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Polaris.WMS.InventoryManage.Application.Inventories
{
    [Authorize(WMSPermissions.InventoryOps.Default)]
    public class InventoryAppService(
        IRepository<Inventory, Guid> inventoryRepository,
        InventoryMappers inventoryMapper,
        InventoryManager inventoryManager,
        IRepository<Container, Guid> containerRepository,
        IProductIntegrationService productIntegrationService,
        ILocationIntegrationService locationIntegrationService,
        IZoneIntegrationService zoneIntegrationService,
        IWarehouseIntegrationService warehouseIntegrationService)
        : ApplicationService, IInventoryAppService
    {
        [Authorize(WMSPermissions.InventoryOps.InventoryViews.View)]
        public async Task<PagedResultDto<InventoryDto>> GetListAsync(InventorySearchDto input)
        {
            var query = await inventoryRepository.GetQueryableAsync();

            if (!string.IsNullOrWhiteSpace(input.ContainerCode))
            {
                var reelQuery = await containerRepository.GetQueryableAsync();
                var containerIdsQuery = reelQuery
                    .Where(x => x.ContainerCode.Contains(input.ContainerCode))
                    .Select(x => x.Id);

                query = query.Where(x => containerIdsQuery.Contains(x.ContainerId));
            }

            if (input.ProductId.HasValue)
            {
                query = query.Where(x => x.ProductId == input.ProductId.Value);
            }

            if (!string.IsNullOrWhiteSpace(input.RelatedOrderNo))
            {
                query = query.Where(x => x.RelatedOrderNo != null && x.RelatedOrderNo.Contains(input.RelatedOrderNo));
            }

            // 跨模块过滤：将原本在本地组装Zone/Warehouse/Location查询，改为呼叫主数据接口逆向查出满足条件的LocationId集合
            if (!string.IsNullOrWhiteSpace(input.zoneCode) || !string.IsNullOrWhiteSpace(input.warehouseCode))
            {
                var locationIds = await locationIntegrationService.GetLocationIdsByCodesAsync(input.zoneCode, input.warehouseCode);

                if (!locationIds.Any())
                {
                    // 如果跨模块查不到库位，直接返回空数据，阻断后续查询
                    return new PagedResultDto<InventoryDto>(0, new List<InventoryDto>());
                }

                var filterReelQuery = await containerRepository.GetQueryableAsync();
                var filterContainerIdsQuery = filterReelQuery
                    .Where(x => x.CurrentLocationId.HasValue && locationIds.Contains(x.CurrentLocationId.Value))
                    .Select(x => x.Id);

                query = query.Where(x => filterContainerIdsQuery.Contains(x.ContainerId));
            }

            var totalCount = await AsyncExecuter.CountAsync(query);

            query = query
                .OrderBy(x => x.ContainerId)
                .ThenByDescending(x => x.LayerIndex)
                .PageBy(input.SkipCount, input.MaxResultCount);

            var items = await AsyncExecuter.ToListAsync(query);

            if (!items.Any())
            {
                return new PagedResultDto<InventoryDto>(totalCount, new List<InventoryDto>());
            }

            var result = items.Select(inventoryMapper.Map).ToList();

            var containerIds = items.Select(x => x.ContainerId).Distinct().ToList();
            var productIds = items.Select(x => x.ProductId).Distinct().ToList();

            // 查 Container (本地模块) 和 Product (跨模块)
            var reelQueryForMap = await containerRepository.GetQueryableAsync();
            var reelsTask = AsyncExecuter.ToListAsync(reelQueryForMap.Where(x => containerIds.Contains(x.Id)));
            var productsTask = productIntegrationService.GetListByIdsAsync(productIds);

            await Task.WhenAll(reelsTask, productsTask);
            var reels = reelsTask.Result;
            var productMap = productsTask.Result.ToDictionary(x => x.Id, x => x);
            var reelMap = reels.ToDictionary(x => x.Id, x => x);

            // 查 Location (跨模块)
            var locationIdsForMap = reels
                .Where(x => x.CurrentLocationId.HasValue)
                .Select(x => x.CurrentLocationId!.Value)
                .Distinct()
                .ToList();

            var locations = await locationIntegrationService.GetListByIdsAsync(locationIdsForMap);
            var locationMap = locations.ToDictionary(x => x.Id, x => x);

            // 查 Zone 和 Warehouse (跨模块)
            var zoneIds = locations.Select(x => x.Id).Distinct().ToList();
            var warehouseIds = locations.Select(x => x.WarehouseId).Distinct().ToList();

            var zonesTask = zoneIntegrationService.GetListByIdsAsync(zoneIds);
            var warehousesTask = warehouseIntegrationService.GetListByIdsAsync(warehouseIds);

            await Task.WhenAll(zonesTask, warehousesTask);
            var zoneMap = zonesTask.Result.ToDictionary(x => x.Id, x => x);
            var warehouseMap = warehousesTask.Result.ToDictionary(x => x.Id, x => x);

            // 内存组装 O(1)
            foreach (var dto in result)
            {
                if (reelMap.TryGetValue(dto.ContainerId, out var container))
                {
                    dto.ContainerCode = container.ContainerCode;

                    if (container.CurrentLocationId.HasValue &&
                        locationMap.TryGetValue(container.CurrentLocationId.Value, out var location))
                    {
                        dto.LocationCode = location.Code;

                        if (zoneMap.TryGetValue(location.Id, out var zone))
                        {
                            dto.ZoneCode = zone.Code;
                            dto.ZoneName = zone.Name;
                        }

                        if (warehouseMap.TryGetValue(location.WarehouseId, out var warehouse))
                        {
                            dto.WarehouseCode = warehouse.Code;
                            dto.WarehouseName = warehouse.Name;
                        }
                    }
                }

                if (productMap.TryGetValue(dto.ProductId, out var product))
                {
                    dto.ProductName = product.Name;
                    dto.ProductCode = product.Code;
                }
            }

            return new PagedResultDto<InventoryDto>(totalCount, result);
        }

        //public async Task<InventoryDto> ProductionReceiveAsync(ProductionReceiveInput input)
        //{
        //    var inventory = await _inventoryManager.ProductionReceiveAsync(
        //        input.ContainerId,
        //        input.ProductId,
        //        input.Quantity,
        //        input.Weight,
        //        input.BatchNo,
        //        input.Source_WO,
        //        input.LocationId,
        //        input.SN,
        //        input.CraftVersion,
        //        input.Unit);

        //    // 閲嶆柊鏌ヨ浠ュ姞杞藉鑸睘鎬?        //    var query = await _inventoryRepository.WithDetailsAsync(
        //        x => x.Container, x => x.Container.CurrentLocation, x => x.Product);

        //    var entity = await AsyncExecuter.FirstOrDefaultAsync(
        //        query.Where(x => x.Id == inventory.Id));

        //    return _inventoryMapper.Map(entity);
        //}
    }
}