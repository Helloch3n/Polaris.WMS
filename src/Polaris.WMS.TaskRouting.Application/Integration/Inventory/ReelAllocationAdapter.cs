using Polaris.WMS.InventoryManage.Application.Contracts.Integration.inventories;
using Polaris.WMS.InventoryManage.Application.Contracts.Integration.Reels;
using Polaris.WMS.TaskRouting.Domain.Integration.Inventory;
using Volo.Abp.DependencyInjection;

namespace Polaris.WMS.TaskRouting.Application.Integration.Inventory;

public class ReelAllocationAdapter(
    IReelIntegrationService reelIntegrationService
) : IReelAllocationAdapter, ITransientDependency
{
    public async Task MoveReelAsync(Guid reelId, Guid targetLocationId)
    {
        await reelIntegrationService.TransferLocationAsync(reelId, targetLocationId);
    }
}