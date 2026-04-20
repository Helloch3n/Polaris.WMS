using Polaris.WMS.Outbound.Application.Contracts.MiscOutboundOrders.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Polaris.WMS.Outbound.Application.Contracts.MiscOutboundOrders;

public interface IMiscOutboundOrderAppService : IApplicationService
{
    /// <summary>
    /// 创建其他出库单。
    /// </summary>
    Task<MiscOutboundOrderDto> CreateAsync(CreateMiscOutboundOrderDto input);

    /// <summary>
    /// 更新其他出库单。
    /// </summary>
    Task<MiscOutboundOrderDto> UpdateAsync(Guid id, UpdateMiscOutboundOrderDto input);

    /// <summary>
    /// 获取其他出库单。
    /// </summary>
    Task<MiscOutboundOrderDto> GetAsync(Guid id);

    /// <summary>
    /// 分页查询其他出库单。
    /// </summary>
    Task<PagedResultDto<MiscOutboundOrderDto>> GetListAsync(MiscOutboundOrderSearchDto input);

    /// <summary>
    /// 删除其他出库单。
    /// </summary>
    Task DeleteAsync(Guid id);
}

