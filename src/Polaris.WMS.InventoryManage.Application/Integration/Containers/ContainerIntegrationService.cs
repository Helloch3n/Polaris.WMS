using Polaris.WMS.InventoryManage.Application.Contracts.Integration.Containers;
using Polaris.WMS.InventoryManage.Domain.Containers;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Polaris.WMS.InventoryManage.Application.Integration.Containers;

[RemoteService(IsEnabled = false)]
public class ContainerIntegrationService(
    ContainerManager containerManager,
    IRepository<Container, Guid> containerRepository
) : ApplicationService, IContainerIntegrationService
{
    public async Task<ContainerIntegrationDto?> GetContainerByCodeAsync(string containerCode)
    {
        var normalizedCode = containerCode?.Trim();
        if (string.IsNullOrWhiteSpace(normalizedCode))
        {
            return null;
        }

        var container = await containerRepository.FirstOrDefaultAsync(x => x.ContainerCode == normalizedCode);
        if (container == null)
        {
            return null;
        }

        return new ContainerIntegrationDto
        {
            Id = container.Id,
            ContainerCode = container.ContainerCode,
            Code = container.ContainerCode,
            Name = container.Name,
            LocationId = container.CurrentLocationId
        };
    }

    public async Task TransferLocationAsync(Guid containerId, Guid scannedLocationId, string? orderNo = null)
    {
        await containerManager.TransferLocationAsync(containerId, scannedLocationId);
    }

    public async Task<List<ContainerIntegrationDto>> GetContainerByLocationIdAsync(Guid locationId)
    {
        // 1. 直接使用 ABP 泛型仓储的条件查询获取列表
        // 假设你的仓储变量名叫 containerRepository
        var containers = await containerRepository.GetListAsync(x => x.CurrentLocationId == locationId);

        // 2. 防护：如果没有查到任何盘具，直接返回空集合
        if (containers == null || !containers.Any())
        {
            return new List<ContainerIntegrationDto>();
        }

        // 3. 映射为集成 DTO 并返回
        return containers.Select(r => new ContainerIntegrationDto
        {
            Id = r.Id,
            LocationId = r.CurrentLocationId
        }).ToList();
    }

    public async Task<ContainerIntegrationDto> GetAsync(Guid id)
    {
        var container = await containerRepository.FirstOrDefaultAsync(x => x.Id == id);
        if (container == null) return null;

        return new ContainerIntegrationDto
        {
            Id = container.Id,
            ContainerCode = container.ContainerCode
        };
    }
}