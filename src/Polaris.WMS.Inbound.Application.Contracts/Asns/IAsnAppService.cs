using Polaris.WMS.Inbound.Application.Contracts.Asns.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Polaris.WMS.Inbound.Application.Contracts.Asns;

public interface IAsnAppService : IApplicationService
{
    Task<AdvancedShippingNoticeDto> GetAsync(Guid id);
    Task<AdvancedShippingNoticeDto> GetByAsnNoAsync(string asnNo);
    Task<PagedResultDto<AdvancedShippingNoticeDto>> GetListAsync(AsnSearchDto input);
}