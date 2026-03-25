using Polaris.WMS.InventoryManage.Application.Contracts.Integration.inventories;
using Polaris.WMS.InventoryManage.Application.Contracts.Integration.Reels;
using Polaris.WMS.TaskRouting.Domain.Integration.Inventory;
using Volo.Abp.DependencyInjection;

namespace Polaris.WMS.TaskRouting.Application.Integration.Inventory;

public class ExternalReelProvider(
    IReelIntegrationService reelIntegrationService
) : IExternalReelProvider, ITransientDependency
{
    public async Task MoveReelAsync(Guid reelId, Guid targetLocationId)
    {
        await reelIntegrationService.TransferLocationAsync(reelId, targetLocationId);
    }
}