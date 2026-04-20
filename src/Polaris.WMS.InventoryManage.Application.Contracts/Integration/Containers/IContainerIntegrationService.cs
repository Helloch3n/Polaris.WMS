using Volo.Abp;
using Volo.Abp.Application.Services;

namespace Polaris.WMS.InventoryManage.Application.Contracts.Integration.Containers;

[RemoteService(IsEnabled = false)]
public interface IContainerIntegrationService : IApplicationService
{
    Task<ContainerIntegrationDto> GetAsync(Guid id);

    Task<ContainerIntegrationDto?> GetContainerByCodeAsync(string containerCode);

    Task TransferLocationAsync(Guid containerId, Guid scannedLocationId, string? orderNo = null);

    Task<List<ContainerIntegrationDto>> GetContainerByLocationIdAsync(Guid locationId);
}