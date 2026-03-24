using Polaris.WMS.InventoryManage.Application.Contracts.TransferOrders.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Polaris.WMS.InventoryManage.Application.Contracts.TransferOrders
{
    /// <summary>
    /// 调拨单应用服务接口。
    /// </summary>
    public interface ITransferAppService : IApplicationService
    {
        /// <summary>
        /// 获取调拨单详情。
        /// </summary>
        Task<TransferDto> GetAsync(Guid id);

        /// <summary>
        /// 分页获取调拨单列表。
        /// </summary>
        Task<PagedResultDto<TransferListDto>> GetListAsync(TransferSearchDto input);

        /// <summary>
        /// 创建调拨单表头。
        /// </summary>
        Task<TransferDto> CreateAsync(CreateTransferDto input);

        /// <summary>
        /// 更新调拨单。
        /// </summary>
        Task<TransferDto> UpdateAsync(TransferDto input);

        /// <summary>
        /// 审核并执行调拨单。
        /// </summary>
        Task ApproveAndExecuteAsync(Guid id);

        /// <summary>
        /// 删除调拨单。
        /// </summary>
        Task DeleteAsync(Guid transferOrderId);
    }
}
