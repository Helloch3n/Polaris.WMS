using Volo.Abp.Application.Dtos;

namespace Polaris.WMS.MasterData.Application.Contracts.Warehouses.Dtos
{
    public class WarehouseSearchDto : PagedAndSortedResultRequestDto
    {
        public string? WarehouseCode { get; set; }
        public string? WarehouseName { get; set; }
    }
}

