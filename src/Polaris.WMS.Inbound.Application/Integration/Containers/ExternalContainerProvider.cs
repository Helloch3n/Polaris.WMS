using Polaris.WMS.Inbound.Domain.Integration.Containers;
using Polaris.WMS.InventoryManage.Application.Contracts.Integration.Containers;
using Volo.Abp.Application.Services;
using Volo.Abp.DependencyInjection;

namespace Polaris.WMS.Inbound.Application.Integration.Containers;

public class ExternalContainerProvider(
    IContainerIntegrationService containerIntegrationService
) : IExternalContainerProvider, ITransientDependency
{
    public Task<List<ExternalContainerInfo>> GetContainerInfosByLocationIdAsync(Guid locationId)
    {
        throw new NotImplementedException();
    }

    public async Task<ExternalContainerInfo?> GetContainerAsync(Guid containerId)
    {
        var dto = await containerIntegrationService.GetAsync(containerId);
        if (dto == null) return null;

        return new ExternalContainerInfo
        {
            Id = dto.Id,
            ContainerCode = dto.ContainerCode
        };
    }

    public async Task MoveContainerAsync(Guid containerId, Guid targetLocationId, string? orderNo = null)
    {
        await containerIntegrationService.TransferLocationAsync(containerId, targetLocationId);
    }
}