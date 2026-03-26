using Polaris.WMS.Inbound.Domain.Integration.Reels;
using Polaris.WMS.InventoryManage.Application.Contracts.Integration.Reels;
using Volo.Abp.Application.Services;
using Volo.Abp.DependencyInjection;

namespace Polaris.WMS.Inbound.Application.Integration.Reels;

public class ExternalReelProvider(
    IReelIntegrationService reelIntegrationService
) : IExternalReelProvider, ITransientDependency
{
    public Task<List<ExternalReelInfo>> GetReelInfosByLocationIdAsync(Guid locationId)
    {
        throw new NotImplementedException();
    }

    public async Task<ExternalReelInfo> GetReelAsync(Guid reelId)
    {
        var dto = await reelIntegrationService.GetAsync(reelId);
        if (dto == null) return null;

        return new ExternalReelInfo
        {
            Id = dto.Id,
            ReelNo = dto.ReelNo
        };
    }

    public async Task MoveReelAsync(Guid reelId, Guid targetLocationId, string? orderNo = null)
    {
        await reelIntegrationService.TransferLocationAsync(reelId, targetLocationId);
    }
}