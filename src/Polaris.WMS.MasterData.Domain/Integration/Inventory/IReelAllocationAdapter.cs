using Volo.Abp.DependencyInjection;

namespace Polaris.WMS.MasterData.Domain.Integration.Inventory;

public interface IReelAllocationAdapter : ITransientDependency
{
    public Task<List<ReelInfo>> GetReelInfosByLocationIdAsync(Guid locationId);
}