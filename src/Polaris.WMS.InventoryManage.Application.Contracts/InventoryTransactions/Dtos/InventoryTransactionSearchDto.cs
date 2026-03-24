using Polaris.WMS.Inventorys;
using Volo.Abp.Application.Dtos;

namespace Polaris.WMS.InventoryManage.Application.Contracts.InventoryTransactions.Dtos
{
    public class InventoryTransactionSearchDto : PagedAndSortedResultRequestDto
    {
        public string? BillNo { get; set; }
        public string? ReelNo { get; set; }
        public Guid? ProductId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public TransactionType? Type { get; set; }
    }
}
