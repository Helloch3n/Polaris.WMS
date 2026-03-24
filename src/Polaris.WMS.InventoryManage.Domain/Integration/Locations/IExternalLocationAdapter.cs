using Volo.Abp.DependencyInjection;

namespace Polaris.WMS.InventoryManage.Domain.Integration.Locations;

public interface IExternalLocationAdapter : ITransientDependency
{
    Task<ExternalLocationInfo> GetLocationAsync(Guid id);
    Task RefreshStatusByLoadAsync(Guid id);
}