using Polaris.WMS.Inbound.Application.Contracts.MiscInboundOrders.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Polaris.WMS.Inbound.Application.Contracts.MiscInboundOrders;

public interface IMiscInboundOrderAppService : IApplicationService
{
    /// <summary>
    /// 创建其他入库单。
    /// </summary>
    Task<MiscInboundOrderDto> CreateAsync(CreateMiscInboundOrderDto input);

    /// <summary>
    /// 更新其他入库单。
    /// </summary>
    Task<MiscInboundOrderDto> UpdateAsync(Guid id, UpdateMiscInboundOrderDto input);

    /// <summary>
    /// 获取其他入库单。
    /// </summary>
    Task<MiscInboundOrderDto> GetAsync(Guid id);

    /// <summary>
    /// 分页查询其他入库单。
    /// </summary>
    Task<PagedResultDto<MiscInboundOrderDto>> GetListAsync(MiscInboundOrderSearchDto input);

    /// <summary>
    /// 删除其他入库单。
    /// </summary>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// 审核并执行其他入库单。
    /// </summary>
    Task ApproveAndExecuteAsync(Guid id);
}

