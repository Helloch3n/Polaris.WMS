using Polaris.WMS.InventoryManage.Domain.Integration.Locations;
using Polaris.WMS.MasterData.Application.Contracts.Integration.Locations;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace Polaris.WMS.InventoryManage.Application.Integration.Locations;

[RemoteService(IsEnabled = false)]
public class ExternalLocationAdapter(
    ILocationIntegrationService locationIntegrationService
) : IExternalLocationAdapter, ITransientDependency
{
    public async Task<ExternalLocationInfo> GetLocationAsync(Guid id)
    {
        var dto = await locationIntegrationService.GetAsync(id);
        return new ExternalLocationInfo
        {
            Id = dto.Id,
            WarehouseId = dto.WarehouseId
        };
    }

    public async Task RefreshStatusByLoadAsync(Guid id)
    {
        await locationIntegrationService.RefreshStatusByLoadAsync(id);
    }
}