using Polaris.WMS.MasterData.Application.Contracts.Zones;
using Polaris.WMS.MasterData.Application.Contracts.Zones.Dtos;
using Polaris.WMS.MasterData.Domain.Locations;
using Polaris.WMS.MasterData.Domain.warehouses;
using Polaris.WMS.MasterData.Domain.Zones;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Polaris.WMS.MasterData.Application.Zones
{
    public class ZoneAppService(
        IRepository<Zone, Guid> zoneRepository,
        IRepository<Warehouse, Guid> warehouseRepository,
        ILocationRepository locationRepository,
        ZoneManager zoneManager)
        : ApplicationService, IZoneAppService
    {
        public async Task<ZoneDto> GetAsync(Guid id)
        {
            var entity = await zoneRepository.GetAsync(id);
            return ObjectMapper.Map<Zone, ZoneDto>(entity);
        }

        public async Task<PagedResultDto<ZoneDto>> GetListAsync(ZoneSearchDto input)
        {
            var query = await zoneRepository.GetQueryableAsync();

            if (!string.IsNullOrWhiteSpace(input.ZoneCode))
            {
                query = query.Where(x => x.Code.Contains(input.ZoneCode));
            }

            if (!string.IsNullOrWhiteSpace(input.ZoneName))
            {
                query = query.Where(x => x.Name.Contains(input.ZoneName));
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

                //var warehouseIds = warehouseQuery.Select(x => x.Id);
                var warehouseIds = await AsyncExecuter.ToListAsync(warehouseQuery.Select(x => x.Id));
                query = query.Where(x => warehouseIds.Contains(x.WarehouseId));
            }

            var totalCount = await AsyncExecuter.CountAsync(query);

            query = query
                .OrderBy(x => x.Code)
                .PageBy(input.SkipCount, input.MaxResultCount);

            var entities = await AsyncExecuter.ToListAsync(query);
            var items = entities.Select(ObjectMapper.Map<Zone, ZoneDto>).ToList();
            return new PagedResultDto<ZoneDto>(totalCount, items);
        }

        public async Task<ZoneDto> CreateAsync(CreateUpdateZoneDto input)
        {
            var entity = await zoneManager.CreateAsync(
                input.WarehouseId,
                input.Code,
                input.Name,
                input.ZoneType);

            return ObjectMapper.Map<Zone, ZoneDto>(entity);
        }

        public async Task<ZoneDto> UpdateAsync(Guid id, CreateUpdateZoneDto input)
        {
            var entity = await zoneRepository.GetAsync(id);

            entity.Update(input.Code, input.Name, input.ZoneType);

            await zoneRepository.UpdateAsync(entity);

            return ObjectMapper.Map<Zone, ZoneDto>(entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            var hasLocations = await locationRepository.AnyAsync(x => x.ZoneId == id);

            if (hasLocations)
            {
                throw new BusinessException("Zone:HasLocations")
                    .WithData("ZoneId", id);
            }

            await zoneRepository.DeleteAsync(id);
        }
    }
}

