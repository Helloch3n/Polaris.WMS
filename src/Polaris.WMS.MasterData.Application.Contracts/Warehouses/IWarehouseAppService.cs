using Polaris.WMS.MasterData.Application.Contracts.Warehouses.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Polaris.WMS.MasterData.Application.Contracts.Warehouses
{
    public interface IWarehouseAppService : IApplicationService
    {
        Task<WarehouseDto> GetAsync(Guid id);
        Task<WarehouseWithDetailsDto> GetWithDetailsAsync(Guid id);
        Task<PagedResultDto<WarehouseDto>> GetListAsync(WarehouseSearchDto input);
        Task<WarehouseDto> CreateAsync(CreateUpdateWarehouseDto input);
        Task<WarehouseDto> UpdateAsync(Guid id, CreateUpdateWarehouseDto input);
        Task DeleteAsync(Guid id);
    }
}

