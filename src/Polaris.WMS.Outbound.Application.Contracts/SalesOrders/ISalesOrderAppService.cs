using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Polaris.WMS.Outbound.Application.Contracts.SalesOrders;

public interface ISalesOrderAppService : IApplicationService
{
    /// <summary>
    /// 创建销售订单。
    /// </summary>
    Task<SalesOrderDto> CreateAsync(CreateSalesOrderDto input);

    /// <summary>
    /// 更新销售订单。
    /// </summary>
    Task<SalesOrderDto> UpdateAsync(Guid id, UpdateSalesOrderDto input);

    /// <summary>
    /// 获取销售订单详情。
    /// </summary>
    Task<SalesOrderDto> GetAsync(Guid id);

    /// <summary>
    /// 分页获取销售订单列表。
    /// </summary>
    Task<PagedResultDto<SalesOrderDto>> GetListAsync(SalesOrderSearchDto input);

    /// <summary>
    /// 删除销售订单。
    /// </summary>
    Task DeleteAsync(Guid id);
}

