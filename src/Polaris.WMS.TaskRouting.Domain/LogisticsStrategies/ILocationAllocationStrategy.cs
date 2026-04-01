using Polaris.WMS.TaskRouting.Domain.Integration.MasterData;

namespace Polaris.WMS.TaskRouting.Domain.LogisticsStrategies;

public interface ILocationAllocationStrategy
{
    Task<ExternalLocationInfo> AllocateLocationAsync(Guid targetZoneId);
}