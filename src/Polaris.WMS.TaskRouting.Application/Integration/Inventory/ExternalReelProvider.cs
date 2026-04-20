using Polaris.WMS.InventoryManage.Application.Contracts.Integration.inventories;
using Polaris.WMS.InventoryManage.Application.Contracts.Integration.Containers;
using Polaris.WMS.TaskRouting.Domain.Integration.Inventory;
using Volo.Abp.DependencyInjection;

namespace Polaris.WMS.TaskRouting.Application.Integration.Inventory;

public class ExternalContainerProvider(
    IContainerIntegrationService containerIntegrationService
) : IExternalContainerProvider, ITransientDependency
{
    public async Task MoveContainerAsync(Guid containerId, Guid targetLocationId)
    {
        await containerIntegrationService.TransferLocationAsync(containerId, targetLocationId);
    }
}