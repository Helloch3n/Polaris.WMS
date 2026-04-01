using Volo.Abp;
using Volo.Abp.Application.Services;

namespace Polaris.WMS.MasterData.Application.Contracts.Integration.Locations;

[RemoteService(IsEnabled = false)]
public interface ILocationIntegrationService : IApplicationService
{
    Task<LocationIntegrationDto> GetBestAvailableLocationIdAsync(Guid zoneId);

    Task<Guid> GetZoneIdByLocationIdAsync(Guid locationId);

    Task RefreshStatusByLoadAsync(Guid id);
    Task<LocationIntegrationDto> GetAsync(Guid id);
    Task<List<Guid>> GetLocationIdsByCodesAsync(string? zoneCode, string? warehouseCode);
    Task<List<LocationIntegrationDto>> GetListByIdsAsync(List<Guid> ids);
    Task<List<Guid>> GetLocationIdsByWarehouseIdAsync(Guid warehouseId);
    Task<LocationIntegrationDto> GetLocationInfoByCodeAsync(string code);
}