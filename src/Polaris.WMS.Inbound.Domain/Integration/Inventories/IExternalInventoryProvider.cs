using Volo.Abp.DependencyInjection;

namespace Polaris.WMS.Inbound.Domain.Integration.Inventories;

public interface IExternalInventoryProvider : ITransientDependency
{
    Task<List<ExternalInventoryInfo>> GetInventoryByReels(List<Guid> reelIds);

    Task ReceiveProductionAsync(ExternalProductionReceiveInfo info);
}