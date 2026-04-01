using Polaris.WMS.MasterData.Application.Contracts.Integration.Locations;
using Polaris.WMS.TaskRouting.Domain.Integration;
using Polaris.WMS.TaskRouting.Domain.Integration.MasterData;
using Volo.Abp.DependencyInjection;

namespace Polaris.WMS.TaskRouting.Application.Integration.MasterData;

public class ExternalLocationProvider(
    ILocationIntegrationService locationIntegrationService // 注入主数据的服务
) : IExternalLocationProvider, ITransientDependency // 实现任务模块的端口
{
    public async Task<ExternalLocationInfo> FindBestLocationIdAsync(Guid zoneId)
    {
        var dto = await locationIntegrationService.GetBestAvailableLocationIdAsync(zoneId);
        return new ExternalLocationInfo
        {
            Id = dto.Id,
            WarehouseId = dto.WarehouseId,
            Code = dto.Code,
            Status = dto.Status
        };
    }

    public async Task<Guid> GetZoneIdByLocationIdAsync(Guid locationId)
    {
        return await locationIntegrationService.GetZoneIdByLocationIdAsync(locationId);
    }

    public async Task<ExternalLocationInfo> GetByCodeAsync(string code)
    {
        var dto = await locationIntegrationService.GetLocationInfoByCodeAsync(code);
        return new ExternalLocationInfo
        {
            Id = dto.Id,
            WarehouseId = dto.WarehouseId,
            Code = dto.Code,
            Status = dto.Status
        };
    }
}