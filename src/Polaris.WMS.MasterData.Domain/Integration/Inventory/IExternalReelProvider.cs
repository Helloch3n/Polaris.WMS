using Volo.Abp.DependencyInjection;

namespace Polaris.WMS.MasterData.Domain.Integration.Inventory;

public interface IExternalContainerProvider : ITransientDependency
{
    public Task<List<ContainerInfo>> GetContainerInfosByLocationIdAsync(Guid locationId);
}