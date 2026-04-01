using Volo.Abp.DependencyInjection;

namespace Polaris.WMS.Inbound.Domain.Integration.MasterData;

public interface ILocationAllocationProvider : ITransientDependency
{
   // Task<Guid> FindBestLocationIdAsync(Guid targetZoneId);
    Task<Guid> GetZoneIdByLocationIdAsync(Guid locationId);
}