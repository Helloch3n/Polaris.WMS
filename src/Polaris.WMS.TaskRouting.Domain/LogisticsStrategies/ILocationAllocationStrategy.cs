namespace Polaris.WMS.TaskRouting.Domain.LogisticsStrategies;

public interface ILocationAllocationStrategy
{
    Task<Guid> AllocateLocationAsync(Guid targetZoneId);
}