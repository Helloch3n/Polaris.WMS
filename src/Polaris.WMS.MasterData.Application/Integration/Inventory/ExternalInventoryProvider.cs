using Polaris.WMS.InventoryManage.Application.Contracts.Integration.inventories;
using Polaris.WMS.MasterData.Domain.Integration.Inventory;
using Volo.Abp;
using Volo.Abp.Application.Services;

namespace Polaris.WMS.MasterData.Application.Integration.Inventory;

[RemoteService(IsEnabled = false)]
public class ExternalInventoryProvider(IInventoryIntegrationService inventoryIntegrationService)
    : ApplicationService, IExternalInventoryProvider
{
    public async Task<List<InventoryInfo>> GetInventoryByContainers(List<Guid> containerIds)
    {
        var inventory = await inventoryIntegrationService.GetInventoryByContainers(containerIds);
        var inventoryInfo = inventory.Select(x => new InventoryInfo
        {
            Id = x.Id,
        }).ToList();
        return inventoryInfo;
    }
}