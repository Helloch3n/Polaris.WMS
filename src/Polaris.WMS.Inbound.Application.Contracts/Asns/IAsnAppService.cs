using Polaris.WMS.Inbound.Application.Contracts.Asns.Dtos;
using Polaris.WMS.Inbound.Application.Contracts.DataSync.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Polaris.WMS.Inbound.Application.Contracts.Asns;

public interface IAsnAppService : IApplicationService
{
    /// <summary>
    /// 同步 (Upsert) 提前发货通知单。
    /// </summary>
    Task SyncAsnAsync(SyncAsnDto input);

    Task<AdvancedShippingNoticeDto> GetAsync(Guid id);
    Task<AdvancedShippingNoticeDto> GetByAsnNoAsync(string asnNo);
    Task<PagedResultDto<AdvancedShippingNoticeDto>> GetListAsync(AsnSearchDto input);
}