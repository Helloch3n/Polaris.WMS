using Polaris.WMS.MasterData.Application.Contracts.Integration.Locations;
using Polaris.WMS.TaskRouting.Domain.Integration;
using Polaris.WMS.TaskRouting.Domain.Integration.MasterData;
using Volo.Abp.DependencyInjection;

namespace Polaris.WMS.TaskRouting.Application.Integration.MasterData;

public class LocationAllocationAdapter(
    ILocationIntegrationService locationIntegrationService // 注入主数据的服务
) : ILocationAllocationAdapter, ITransientDependency // 实现任务模块的端口
{
    public async Task<Guid> FindBestLocationIdAsync(Guid productId)
    {
        return await locationIntegrationService.GetBestAvailableLocationIdAsync(productId);
    }

    public async Task<Guid> GetZoneIdByLocationIdAsync(Guid locationId)
    {
        return await locationIntegrationService.GetZoneIdByLocationIdAsync(locationId);
    }
}