using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Polaris.WMS.Outbound.Application.Contracts.SalesShipments;

public interface ISalesShipmentAppService : IApplicationService
{
    /// <summary>
    /// 创建销售发货单。
    /// </summary>
    Task<SalesShipmentDto> CreateAsync(CreateSalesShipmentDto input);

    /// <summary>
    /// 更新销售发货单。
    /// </summary>
    Task<SalesShipmentDto> UpdateAsync(Guid id, UpdateSalesShipmentDto input);

    /// <summary>
    /// 获取销售发货单详情。
    /// </summary>
    Task<SalesShipmentDto> GetAsync(Guid id);

    /// <summary>
    /// 分页获取销售发货单列表。
    /// </summary>
    Task<PagedResultDto<SalesShipmentDto>> GetListAsync(SalesShipmentSearchDto input);

    /// <summary>
    /// 删除销售发货单。
    /// </summary>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// 为销售发货明细新增分配记录。
    /// </summary>
    Task<SalesShipmentAllocationDto> AddAllocationAsync(CreateSalesShipmentAllocationDto input);

    /// <summary>
    /// 释放销售发货明细的分配记录。
    /// </summary>
    Task RemoveAllocationAsync(Guid salesShipmentId, Guid detailId, Guid allocationId);

    /// <summary>
    /// 确认销售发货分配记录已完成拣货。
    /// </summary>
    Task<SalesShipmentAllocationDto> MarkAllocationPickedAsync(MarkSalesShipmentAllocationPickedDto input);

    /// <summary>
    /// 向销售发货明细新增发货记录。
    /// </summary>
    Task<SalesShipmentRecordDto> AddRecordsAsync(AddSalesShipmentRecordsDto input);

    /// <summary>
    /// 审核并执行销售发货单。
    /// </summary>
    Task ApproveAndExecuteAsync(Guid id);
}


