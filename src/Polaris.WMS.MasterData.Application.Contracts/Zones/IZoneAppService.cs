using Polaris.WMS.MasterData.Application.Contracts.Zones.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Polaris.WMS.MasterData.Application.Contracts.Zones
{
    public interface IZoneAppService : IApplicationService
    {
        Task<ZoneDto> GetAsync(Guid id);
        Task<PagedResultDto<ZoneDto>> GetListAsync(ZoneSearchDto input);
        Task<ZoneDto> CreateAsync(CreateUpdateZoneDto input);
        Task<ZoneDto> UpdateAsync(Guid id, CreateUpdateZoneDto input);
        Task DeleteAsync(Guid id);
    }
}

