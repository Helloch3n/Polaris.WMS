using Volo.Abp.DependencyInjection;

namespace Polaris.WMS.TaskRouting.Domain.Integration.MasterData;

public interface IExternalLocationProvider : ITransientDependency
{
    Task<Guid> FindBestLocationIdAsync(Guid targetZoneId);
    Task<Guid> GetZoneIdByLocationIdAsync(Guid locationId);
}