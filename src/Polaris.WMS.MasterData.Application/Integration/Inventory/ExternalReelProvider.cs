using Polaris.WMS.InventoryManage.Application.Contracts.Integration.inventories;
using Polaris.WMS.InventoryManage.Application.Contracts.Integration.Reels;
using Polaris.WMS.MasterData.Domain.Integration.Inventory;
using Volo.Abp.Application.Services;

namespace Polaris.WMS.MasterData.Application.Integration.Inventory;

public class ExternalReelProvider(IReelIntegrationService reelIntegrationService)
    : ApplicationService, IExternalReelProvider
{
    public async Task<List<ReelInfo>> GetReelInfosByLocationIdAsync(Guid locationId)
    {
        var reels = await reelIntegrationService.GetReelByLocationIdAsync(locationId);
        var reelInfos = reels.Select(x => new ReelInfo
        {
            Id = x.Id,
            Code = x.Code,
            LocationId = x.LocationId,
            Name = x.Name
        }).ToList();
        return reelInfos;
    }
}