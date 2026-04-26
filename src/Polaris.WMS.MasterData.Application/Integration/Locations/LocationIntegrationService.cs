using Polaris.WMS.MasterData.Application.Contracts.Integration.Locations;
using Polaris.WMS.MasterData.Domain.Locations;
using Polaris.WMS.MasterData.Domain.warehouses;
using Polaris.WMS.MasterData.Domain.Zones;
using Polaris.WMS.MasterData.Warehouses;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Polaris.WMS.MasterData.Application.Integration.Locations;

[RemoteService(IsEnabled = false)]
public class LocationIntegrationService(
    IRepository<Location, Guid> locationRepository,
    IRepository<Zone, Guid> zoneRepository,
    IRepository<Warehouse, Guid> warehouseRepository,
    LocationManager locationManager
) : ApplicationService, ILocationIntegrationService
{
    public async Task<LocationIntegrationDto> GetBestAvailableLocationIdAsync(Guid zoneId)
    {
        // 查找可用库位
        var location = await locationRepository.FirstOrDefaultAsync(x =>
            x.ZoneId == zoneId &&
            (x.Status == LocationStatus.Idle || x.Status == LocationStatus.Partial)); // 举个例子

        if (location == null)
        {
            throw new UserFriendlyException($"在库区 {zoneId} 中没有找到任何可用（空闲或部分占用）的库位！");
        }

        return new LocationIntegrationDto
        {
            Id = location.Id,
            Code = location.Code,
            WarehouseId = location.WarehouseId
        };
    }

    public async Task<Guid> GetZoneIdByLocationIdAsync(Guid locationId)
    {
        var location = await locationRepository.GetAsync(x => x.Id == locationId);
        return location.ZoneId;
    }

    public async Task<LocationIntegrationDto> GetAsync(Guid id)
    {
        var location = await locationRepository.GetAsync(id);
        if (location == null)
        {
            throw new UserFriendlyException($"未找到Id为{id}的库位");
        }

        return new LocationIntegrationDto
        {
            Id = location.Id,
            WarehouseId = location.WarehouseId,
            Code = location.Code,
            Status = location.Status
        };
    }

    public async Task<List<Guid>> GetLocationIdsByCodesAsync(string? zoneCode, string? warehouseCode)
    {
        var locationQuery = await locationRepository.GetQueryableAsync();

        if (!string.IsNullOrWhiteSpace(zoneCode))
        {
            var zoneQuery = await zoneRepository.GetQueryableAsync();
            var zoneIds = zoneQuery.Where(x => x.Code.Contains(zoneCode)).Select(x => x.Id);
            locationQuery = locationQuery.Where(x => zoneIds.Contains(x.ZoneId));
        }

        if (!string.IsNullOrWhiteSpace(warehouseCode))
        {
            var warehouseQuery = await warehouseRepository.GetQueryableAsync();
            var warehouseIds = warehouseQuery.Where(x => x.Code.Contains(warehouseCode)).Select(x => x.Id);
            locationQuery = locationQuery.Where(x => warehouseIds.Contains(x.WarehouseId));
        }

        // 只需要返回 ID 即可
        return await AsyncExecuter.ToListAsync(locationQuery.Select(x => x.Id));
    }

    public async Task<List<LocationIntegrationDto>> GetListByIdsAsync(List<Guid> ids)
    {
        if (ids == null || !ids.Any()) return new List<LocationIntegrationDto>();
        var query = await locationRepository.GetQueryableAsync();
        var list = await AsyncExecuter.ToListAsync(query.Where(x => ids.Contains(x.Id)));

        return list.Select(x => new LocationIntegrationDto
        {
            Id = x.Id,
            Code = x.Code,
            Status = x.Status,
            WarehouseId = x.WarehouseId
        }).ToList();
    }

    public async Task RefreshStatusByLoadAsync(Guid id)
    {
        await locationManager.RefreshStatusByLoadAsync(id);
    }

    public async Task<List<Guid>> GetLocationIdsByWarehouseIdAsync(Guid warehouseId)
    {
        var query = await locationRepository.GetQueryableAsync();
        return await AsyncExecuter.ToListAsync(query
            .Where(x => x.WarehouseId == warehouseId)
            .Select(x => x.Id));
    }

    public async Task<LocationIntegrationDto> GetLocationInfoByCodeAsync(string code)
    {
        var query = await locationRepository.GetQueryableAsync();

        var location = await AsyncExecuter.FirstOrDefaultAsync(query
            .Where(x => x.Code == code));

        if (location == null)
        {
            throw new UserFriendlyException($"系统不存在编码为 '{code}' 的库位!");
        }

        return new LocationIntegrationDto()
        {
            Id = location.Id,
            Code = location.Code,
            WarehouseId = location.WarehouseId,
            Status = location.Status
        };
    }
}