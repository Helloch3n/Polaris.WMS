using Volo.Abp.DependencyInjection;

namespace Polaris.WMS.MasterData.Domain.Integration.Inventory;

public interface IInventoryAllocationAdapter : ITransientDependency
{
    Task<List<InventoryInfo>> GetInventoryByReels(List<Guid> reelIds);
}