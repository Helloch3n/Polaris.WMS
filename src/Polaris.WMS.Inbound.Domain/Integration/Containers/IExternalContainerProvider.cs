using Volo.Abp.DependencyInjection;

namespace Polaris.WMS.Inbound.Domain.Integration.Containers;

public interface IExternalContainerProvider : ITransientDependency
{
    public Task<List<ExternalContainerInfo>> GetContainerInfosByLocationIdAsync(Guid locationId);

    Task<ExternalContainerInfo?> GetContainerAsync(Guid containerId);

    Task MoveContainerAsync(Guid containerId, Guid targetLocationId, string? orderNo = null);
}