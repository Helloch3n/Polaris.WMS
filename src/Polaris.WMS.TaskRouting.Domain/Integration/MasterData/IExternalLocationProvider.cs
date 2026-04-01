using Volo.Abp.DependencyInjection;

namespace Polaris.WMS.TaskRouting.Domain.Integration.MasterData;

public interface IExternalLocationProvider : ITransientDependency
{
    Task<ExternalLocationInfo> FindBestLocationIdAsync(Guid targetZoneId);
    Task<Guid> GetZoneIdByLocationIdAsync(Guid locationId);
    Task<ExternalLocationInfo> GetByCodeAsync(string code);
}