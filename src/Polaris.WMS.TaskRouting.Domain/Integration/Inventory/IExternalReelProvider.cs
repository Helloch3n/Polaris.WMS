using Volo.Abp.DependencyInjection;

namespace Polaris.WMS.TaskRouting.Domain.Integration.Inventory;

public interface IExternalReelProvider : ITransientDependency
{
    Task MoveReelAsync(Guid reelId, Guid targetLocationId);
}