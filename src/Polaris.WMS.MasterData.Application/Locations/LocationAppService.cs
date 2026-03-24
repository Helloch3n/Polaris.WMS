using Polaris.WMS.MasterData.Application.Contracts.Locations;
using Polaris.WMS.MasterData.Application.Contracts.Locations.Dtos;
using Polaris.WMS.MasterData.Domain.Locations;
using Polaris.WMS.MasterData.Domain.warehouses;
using Polaris.WMS.MasterData.Domain.Zones;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Polaris.WMS.MasterData.Application.Locations
{
    public class LocationAppService(
        IRepository<Location, Guid> locationRepository,
        IRepository<Zone, Guid> zoneRepository,
        IRepository<Warehouse, Guid> warehouseRepository,
        LocationManager locationManager)
        : ApplicationService, ILocationAppService
    {
        public async Task<LocationDto> GetAsync(Guid id)
        {
            var entity = await locationRepository.GetAsync(id);
            return ObjectMapper.Map<Location, LocationDto>(entity);
        }

        public async Task<PagedResultDto<LocationDto>> GetListAsync(LocationSearchDto input)
        {
            var query = await locationRepository.GetQueryableAsync();

            if (!string.IsNullOrWhiteSpace(input.LocationCode))
            {
                query = query.Where(x => x.Code.Contains(input.LocationCode));
            }

            if (!string.IsNullOrWhiteSpace(input.ZoneCode) || !string.IsNullOrWhiteSpace(input.ZoneName))
            {
                var zoneQuery = await zoneRepository.GetQueryableAsync();

                if (!string.IsNullOrWhiteSpace(input.ZoneCode))
                {
                    zoneQuery = zoneQuery.Where(x => x.Code.Contains(input.ZoneCode));
                }

                if (!string.IsNullOrWhiteSpace(input.ZoneName))
                {
                    zoneQuery = zoneQuery.Where(x => x.Name.Contains(input.ZoneName));
                }

                var zoneIds = zoneQuery.Select(x => x.Id);
                query = query.Where(x => zoneIds.Contains(x.ZoneId));
            }

            if (!string.IsNullOrWhiteSpace(input.WarehouseCode) || !string.IsNullOrWhiteSpace(input.WarehouseName))
            {
                var warehouseQuery = await warehouseRepository.GetQueryableAsync();

                if (!string.IsNullOrWhiteSpace(input.WarehouseCode))
                {
                    warehouseQuery = warehouseQuery.Where(x => x.Code.Contains(input.WarehouseCode));
                }

                if (!string.IsNullOrWhiteSpace(input.WarehouseName))
                {
                    warehouseQuery = warehouseQuery.Where(x => x.Name.Contains(input.WarehouseName));
                }

                var warehouseIds = warehouseQuery.Select(x => x.Id);
                query = query.Where(x => warehouseIds.Contains(x.WarehouseId));
            }

            var totalCount = await AsyncExecuter.CountAsync(query);

            query = query
                .OrderBy(x => x.Code)
                .PageBy(input.SkipCount, input.MaxResultCount);

            var entities = await AsyncExecuter.ToListAsync(query);
            var items = entities.Select(ObjectMapper.Map<Location, LocationDto>).ToList();
            return new PagedResultDto<LocationDto>(totalCount, items);
        }

        public async Task<LocationDto> CreateAsync(CreateUpdateLocationDto input)
        {
            var entity = await locationManager.CreateAsync(
                input.WarehouseId,
                input.ZoneId,
                input.Code,
                input.Aisle,
                input.Rack,
                input.Level,
                input.Bin,
                input.MaxWeight,
                input.MaxVolume,
                input.MaxReelCount,
                input.Type,
                input.AllowMixedProducts,
                input.AllowMixedBatches);

            return ObjectMapper.Map<Location, LocationDto>(entity);
        }

        public async Task<LocationDto> UpdateAsync(Guid id, CreateUpdateLocationDto input)
        {
            var entity = await locationRepository.GetAsync(id);

            entity.UpdateBasicInfo(
                input.Code,
                input.Aisle,
                input.Rack,
                input.Level,
                input.Bin,
                input.MaxWeight,
                input.MaxVolume,
                input.MaxReelCount,
                input.Type,
                input.AllowMixedProducts,
                input.AllowMixedBatches);

            await locationRepository.UpdateAsync(entity);

            return ObjectMapper.Map<Location, LocationDto>(entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            await locationRepository.DeleteAsync(id);
        }

        public async Task EnsureExistsAsync(Guid id)
        {
            var exists = await locationRepository.AnyAsync(x => x.Id == id);
            if (!exists)
            {
                throw new BusinessException("Reel:LocationNotFound")
                    .WithData("LocationId", id);
            }
        }

        public async Task<ListResultDto<LocationDto>> BatchCreateAsync(BatchCreateLocationDto input)
        {
            var entities = await locationManager.BatchCreateAsync(
                input.WarehouseId,
                input.ZoneId,
                input.AislePrefix,
                input.RackCount,
                input.LevelCount);

            var items = entities.Select(ObjectMapper.Map<Location, LocationDto>).ToList();
            return new ListResultDto<LocationDto>(items);
        }

        /// <summary>
        /// 根据仓库Id获取库位列表。
        /// </summary>
        public async Task<ListResultDto<LocationDto>> GetLocationByWarehouseIdAsync(Guid warehouseId)
        {
            if (warehouseId == Guid.Empty)
            {
                throw new BusinessException("仓库Id不能为空");
            }

            // 第一步：查询仓库下所有库位
            var query = await locationRepository.GetQueryableAsync();
            var entities = await AsyncExecuter.ToListAsync(
                query
                    .Where(x => x.WarehouseId == warehouseId)
                    .OrderBy(x => x.Code));

            // 第二步：映射并返回
            var items = entities.Select(ObjectMapper.Map<Location, LocationDto>).ToList();
            return new ListResultDto<LocationDto>(items);
        }
    }
}

