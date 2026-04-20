using Polaris.WMS.InventoryManage.Application.Contracts.Integration.inventories;
using Polaris.WMS.InventoryManage.Application.Contracts.Integration.Containers;
using Polaris.WMS.MasterData.Domain.Integration.Inventory;
using Volo.Abp.Application.Services;

namespace Polaris.WMS.MasterData.Application.Integration.Inventory;

public class ExternalContainerProvider(IContainerIntegrationService containerIntegrationService)
    : ApplicationService, IExternalContainerProvider
{
    public async Task<List<ContainerInfo>> GetContainerInfosByLocationIdAsync(Guid locationId)
    {
        var containers = await containerIntegrationService.GetContainerByLocationIdAsync(locationId);
        var containerInfos = containers.Select(x => new ContainerInfo
        {
            Id = x.Id,
            Code = x.Code,
            LocationId = x.LocationId,
            Name = x.Name
        }).ToList();
        return containerInfos;
    }
}