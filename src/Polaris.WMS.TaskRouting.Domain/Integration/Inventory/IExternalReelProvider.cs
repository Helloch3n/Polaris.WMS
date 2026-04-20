using Volo.Abp.DependencyInjection;

namespace Polaris.WMS.TaskRouting.Domain.Integration.Inventory;

public interface IExternalContainerProvider : ITransientDependency
{
    Task MoveContainerAsync(Guid containerId, Guid targetLocationId);
}