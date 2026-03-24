using Volo.Abp.Application.Services;

namespace Polaris.WMS.MasterData.Application.Contracts.Integration.Zones;

public interface IZoneIntegrationService: IApplicationService
{
    Task<List<ZoneIntegrationDto>> GetListByIdsAsync(List<Guid> ids);
}