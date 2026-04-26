using Polaris.WMS.Inbound.Domain.Integration.MasterData;
using Polaris.WMS.MasterData.Application.Contracts.Integration.Locations;
using Polaris.WMS.MasterData.Warehouses;
using Volo.Abp.DependencyInjection;

namespace Polaris.WMS.Inbound.Application.Integration.MasterData;

public class LocationAllocationProvider(
    ILocationIntegrationService locationIntegrationService // 注入主数据的服务
) : ILocationAllocationProvider, ITransientDependency // 实现任务模块的端口
{
    // public async Task<Guid> FindBestLocationIdAsync(Guid productId)
    // {
    //     return await locationIntegrationService.GetBestAvailableLocationIdAsync(productId);
    // }

    public async Task<Guid> GetZoneIdByLocationIdAsync(Guid locationId)
    {
        return await locationIntegrationService.GetZoneIdByLocationIdAsync(locationId);
    }

    public async Task<bool> IsLockedAsync(Guid locationId)
    {
        var location = await locationIntegrationService.GetAsync(locationId);
        return location.Status == LocationStatus.Locked;
    }
}