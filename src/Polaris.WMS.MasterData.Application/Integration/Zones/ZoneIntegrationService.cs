using Polaris.WMS.MasterData.Application.Contracts.Integration.Zones;
using Polaris.WMS.MasterData.Domain.Zones;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Polaris.WMS.MasterData.Application.Integration.Zones;

[RemoteService(IsEnabled = false)]
public class ZoneIntegrationService(
    IRepository<Zone, Guid> zoneRepository)
    : ApplicationService, IZoneIntegrationService
{
    public async Task<List<ZoneIntegrationDto>> GetListByIdsAsync(List<Guid> ids)
    {
        if (ids == null || !ids.Any()) return new List<ZoneIntegrationDto>();
        var query = await zoneRepository.GetQueryableAsync();
        var list = await AsyncExecuter.ToListAsync(query.Where(x => ids.Contains(x.Id)));

        return list.Select(x => new ZoneIntegrationDto { Id = x.Id, Code = x.Code, Name = x.Name }).ToList();
    }
}