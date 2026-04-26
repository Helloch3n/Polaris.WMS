using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Polaris.WMS.Outbound.Application.Contracts.Reviews;

public interface IOutboundReviewOrderAppService : IApplicationService
{
    /// <summary>
    /// 创建出库复核单。
    /// </summary>
    Task<OutboundReviewOrderDto> CreateAsync(CreateOutboundReviewOrderDto input);

    /// <summary>
    /// 完成出库复核单。
    /// </summary>
    Task CompleteAsync(Guid id);

    /// <summary>
    /// 获取出库复核单详情。
    /// </summary>
    Task<OutboundReviewOrderDto> GetAsync(Guid id);

    /// <summary>
    /// 分页获取出库复核单列表。
    /// </summary>
    Task<PagedResultDto<OutboundReviewOrderDto>> GetListAsync(OutboundReviewOrderSearchDto input);
}

