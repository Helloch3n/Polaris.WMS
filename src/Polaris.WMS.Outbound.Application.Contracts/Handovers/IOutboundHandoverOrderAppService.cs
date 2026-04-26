using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Polaris.WMS.Outbound.Application.Contracts.Handovers;

public interface IOutboundHandoverOrderAppService : IApplicationService
{
    /// <summary>
    /// 创建出库交接单。
    /// </summary>
    Task<OutboundHandoverOrderDto> CreateAsync(CreateOutboundHandoverOrderDto input);

    /// <summary>
    /// 完成交接单并执行销售发货。
    /// </summary>
    Task CompleteAsync(Guid id);

    /// <summary>
    /// 获取出库交接单详情。
    /// </summary>
    Task<OutboundHandoverOrderDto> GetAsync(Guid id);

    /// <summary>
    /// 分页获取出库交接单列表。
    /// </summary>
    Task<PagedResultDto<OutboundHandoverOrderDto>> GetListAsync(OutboundHandoverOrderSearchDto input);
}

