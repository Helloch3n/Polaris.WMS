using Volo.Abp;
using Volo.Abp.Application.Services;

namespace Polaris.WMS.MasterData.Application.Contracts.Integration.Warehouses;

[RemoteService(IsEnabled = false)]
public interface IWarehouseIntegrationService : IApplicationService
{
    Task<WarehouseIntegrationDto> GetAsync(Guid id);

    Task<List<WarehouseIntegrationDto>> GetListByIdsAsync(List<Guid> ids);
}