using Volo.Abp.Application.Dtos;

namespace Polaris.WMS.MasterData.Application.Contracts.Products.Dtos
{
    public class ProductSearchDto : PagedAndSortedResultRequestDto
    {
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
    }
}

