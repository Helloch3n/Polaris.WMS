using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Polaris.WMS.Outbound.Application.Contracts.PickLists;

public interface IPickListAppService : IApplicationService
{
    /// <summary>
    /// 创建拣货单。
    /// </summary>
    Task<PickListDto> CreateAsync(CreatePickListDto input);

    /// <summary>
    /// 释放拣货单并创建拣货任务。
    /// </summary>
    Task ReleaseAsync(Guid id);

    /// <summary>
    /// 获取拣货单详情。
    /// </summary>
    Task<PickListDto> GetAsync(Guid id);

    /// <summary>
    /// 分页获取拣货单列表。
    /// </summary>
    Task<PagedResultDto<PickListDto>> GetListAsync(PickListSearchDto input);
}

