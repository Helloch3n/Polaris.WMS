using Volo.Abp.DependencyInjection;

namespace Polaris.WMS.Inbound.Domain.Integration.Reels;

public interface IExternalReelProvider : ITransientDependency
{
    public Task<List<ExternalReelInfo>> GetReelInfosByLocationIdAsync(Guid locationId);

    Task<ExternalReelInfo> GetReelAsync(Guid reelId);

    Task MoveReelAsync(Guid reelId, Guid targetLocationId, string? orderNo = null);
}