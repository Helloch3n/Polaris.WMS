using Polaris.WMS.InventoryManage.Application.Contracts.Reels.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Polaris.WMS.InventoryManage.Application.Contracts.Reels
{
    public interface IReelAppService :
        ICrudAppService<ReelDto, Guid, ReelSearchDto, CreateUpdateReelDto>
    {
        Task UpdateLocationAsync(Guid id, Guid newLocationId);
        Task<PagedResultDto<ReelDto>> GetListAsync(ReelSearchDto input);
        Task<List<MovableReelDto>> GetMovableReelsAsync(Guid sourceWarehouseId);
    }
}
