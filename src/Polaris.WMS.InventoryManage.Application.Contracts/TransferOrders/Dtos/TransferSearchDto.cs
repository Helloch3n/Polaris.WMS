using Polaris.WMS.TransferOrders;
using Volo.Abp.Application.Dtos;

namespace Polaris.WMS.InventoryManage.Application.Contracts.TransferOrders.Dtos
{
    /// <summary>
    /// 调拨单查询输入。
    /// </summary>
    public class TransferSearchDto : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// 调拨单号。
        /// </summary>
        public string? OrderNo { get; set; }

        /// <summary>
        /// 调拨单状态。
        /// </summary>
        public TransferOrderStatus? Status { get; set; }

        /// <summary>
        /// 开始创建时间。
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 结束创建时间。
        /// </summary>
        public DateTime? EndTime { get; set; }
    }
}
