using Volo.Abp.DependencyInjection;

namespace Polaris.WMS.Inbound.Domain.ProductionInbounds.Integration.MasterData;

public interface ILocationAllocationProvider : ITransientDependency
{
    Task<Guid> FindBestLocationIdAsync(Guid targetZoneId);
    Task<Guid> GetZoneIdByLocationIdAsync(Guid locationId);
}