using Volo.Abp.Application.Dtos;

namespace Polaris.WMS.InventoryManage.Application.Contracts.Inventories.Dtos
{
    public class InventorySearchDto : PagedAndSortedResultRequestDto
    {
        public string? ReelNo { get; set; }
        public Guid? ProductId { get; set; }
        public string? RelatedOrderNo { get; set; }
        public string? zoneCode { get; set; }
        public string? warehouseCode { get; set; }
    }
}
