using Polaris.WMS.MasterData.Application.Contracts.Locations.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Polaris.WMS.MasterData.Application.Contracts.Locations
{
    public interface ILocationAppService : IApplicationService
    {
        Task<LocationDto> GetAsync(Guid id);
        Task<PagedResultDto<LocationDto>> GetListAsync(LocationSearchDto input);
        Task<LocationDto> CreateAsync(CreateUpdateLocationDto input);
        Task<LocationDto> UpdateAsync(Guid id, CreateUpdateLocationDto input);
        Task DeleteAsync(Guid id);
        Task<ListResultDto<LocationDto>> BatchCreateAsync(BatchCreateLocationDto input);
        Task<ListResultDto<LocationDto>> GetLocationByWarehouseIdAsync(Guid warehouseId);
        Task EnsureExistsAsync(Guid id);
    }
}

