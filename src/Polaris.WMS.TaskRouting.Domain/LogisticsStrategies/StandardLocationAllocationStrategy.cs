using Polaris.WMS.MasterData.Warehouses;
using Polaris.WMS.TaskRouting.Domain.Integration;
using Polaris.WMS.TaskRouting.Domain.Integration.MasterData;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace Polaris.WMS.TaskRouting.Domain.LogisticsStrategies;

public class StandardLocationAllocationStrategy(
    IExternalLocationProvider externalLocationProvider)
    : ILocationAllocationStrategy, ITransientDependency
{
    public async Task<Guid> AllocateLocationAsync(Guid targetZoneId)
    {
        var locationId = await externalLocationProvider.FindBestLocationIdAsync(targetZoneId);
        return locationId;
    }
}