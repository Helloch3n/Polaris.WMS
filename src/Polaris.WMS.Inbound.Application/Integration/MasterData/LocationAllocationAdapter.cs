using Polaris.WMS.Inbound.Domain.ProductionInbounds.Integration.MasterData;
using Polaris.WMS.MasterData.Application.Contracts.Integration.Locations;
using Volo.Abp.DependencyInjection;

namespace Polaris.WMS.Inbound.Application.Integration.MasterData;

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