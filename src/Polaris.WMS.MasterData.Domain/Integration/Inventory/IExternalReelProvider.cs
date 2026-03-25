using Volo.Abp.DependencyInjection;

namespace Polaris.WMS.MasterData.Domain.Integration.Inventory;

public interface IExternalReelProvider : ITransientDependency
{
    public Task<List<ReelInfo>> GetReelInfosByLocationIdAsync(Guid locationId);
}