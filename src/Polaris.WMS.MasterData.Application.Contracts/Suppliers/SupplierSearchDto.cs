using Volo.Abp.Application.Dtos;

namespace Polaris.WMS.MasterData.Application.Contracts.Suppliers
{
    public class SupplierSearchDto : PagedAndSortedResultRequestDto
    {
        public string? SupplierCode { get; set; }
        public string? SupplierName { get; set; }
    }
}

