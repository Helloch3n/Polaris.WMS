using Polaris.WMS.MasterData.Application.Contracts.Integration.Warehouses;
using Polaris.WMS.MasterData.Domain.warehouses;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Polaris.WMS.MasterData.Application.Integration.Warehouses;

[RemoteService(IsEnabled = false)]
public class WarehouseIntegrationService(
    IRepository<Warehouse, Guid> warehouseRepository
) : ApplicationService, IWarehouseIntegrationService
{
    public async Task<WarehouseIntegrationDto> GetAsync(Guid id)
    {
        var warehouse = await warehouseRepository.FindAsync(id);
        if (warehouse == null) return null;

        return new WarehouseIntegrationDto
        {
            Id = warehouse.Id,
            Name = warehouse.Name,
            Code = warehouse.Code
        };
    }

    public async Task<List<WarehouseIntegrationDto>> GetListByIdsAsync(List<Guid> ids)
    {
        if (ids == null || !ids.Any()) return new List<WarehouseIntegrationDto>();
        var query = await warehouseRepository.GetQueryableAsync();
        var list = await AsyncExecuter.ToListAsync(query.Where(x => ids.Contains(x.Id)));
        return list.Select(x => new WarehouseIntegrationDto { Id = x.Id, Name = x.Name, Code = x.Code }).ToList();
    }
}