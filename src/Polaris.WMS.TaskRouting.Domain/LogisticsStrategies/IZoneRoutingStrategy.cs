using Polaris.WMS.Tasks;

namespace Polaris.WMS.TaskRouting.Domain.LogisticsStrategies;

public interface IZoneRoutingStrategy
{
    Task<Guid> CalculateTargetZoneAsync(MoveTaskType taskType,  Guid currentLocationId);
}