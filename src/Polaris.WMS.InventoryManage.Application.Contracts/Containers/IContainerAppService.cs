using Polaris.WMS.InventoryManage.Application.Contracts.Containers.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Polaris.WMS.InventoryManage.Application.Contracts.Containers
{
    public interface IContainerAppService :
        ICrudAppService<ContainerDto, Guid, ContainerSearchDto, CreateUpdateContainerDto>
    {
        Task UpdateLocationAsync(Guid id, Guid newLocationId);
        Task<PagedResultDto<ContainerDto>> GetListAsync(ContainerSearchDto input);
        Task<List<MovableContainerDto>> GetMovableReelsAsync(Guid sourceWarehouseId);
    }
}
