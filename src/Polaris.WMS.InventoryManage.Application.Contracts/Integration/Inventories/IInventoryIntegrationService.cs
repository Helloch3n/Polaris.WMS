using Volo.Abp;
using Volo.Abp.Application.Services;

namespace Polaris.WMS.InventoryManage.Application.Contracts.Integration.inventories;

[RemoteService(IsEnabled = false)]
public interface IInventoryIntegrationService : IApplicationService
{
    public Task<List<InventoryIntegrationDto>> GetInventoryByContainers(List<Guid> containerIds);
    Task ReceiveProductionAsync(ProductionReceiveByContainerInput input);
    Task ShipAsync(ShipByContainerInput input);
}