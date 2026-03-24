using Polaris.WMS.InventoryManage.Application.Contracts.Integration.inventories;
using Polaris.WMS.MasterData.Domain.Integration.Inventory;
using Volo.Abp;
using Volo.Abp.Application.Services;

namespace Polaris.WMS.MasterData.Application.Integration.Inventory;

[RemoteService(IsEnabled = false)]
public class InventoryAllocationAdapter(IInventoryIntegrationService inventoryIntegrationService)
    : ApplicationService, IInventoryAllocationAdapter
{
    public async Task<List<InventoryInfo>> GetInventoryByReels(List<Guid> reelIds)
    {
        var inventory = await inventoryIntegrationService.GetInventoryByReels(reelIds);
        var inventoryInfo = inventory.Select(x => new InventoryInfo
        {
            Id = x.Id,
        }).ToList();
        return inventoryInfo;
    }
}