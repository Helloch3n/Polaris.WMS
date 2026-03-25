using Volo.Abp.DependencyInjection;

namespace Polaris.WMS.MasterData.Domain.Integration.Inventory;

public interface IExternalInventoryProvider : ITransientDependency
{
    Task<List<InventoryInfo>> GetInventoryByReels(List<Guid> reelIds);
}