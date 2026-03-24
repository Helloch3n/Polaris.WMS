using Volo.Abp.DependencyInjection;

namespace Polaris.WMS.TaskRouting.Domain.Integration.Inventory;

public interface IReelAllocationAdapter : ITransientDependency
{
    Task MoveReelAsync(Guid reelId, Guid targetLocationId);
}