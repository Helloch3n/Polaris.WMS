using Volo.Abp.Application.Dtos;

namespace Polaris.WMS.InventoryManage.Application.Contracts.Reels.Dtos
{
    public class ReelSearchDto : PagedAndSortedResultRequestDto
    {
        public string? ReelCode { get; set; }
    }
}

