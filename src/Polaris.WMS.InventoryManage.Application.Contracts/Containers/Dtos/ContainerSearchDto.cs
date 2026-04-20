using Volo.Abp.Application.Dtos;

namespace Polaris.WMS.InventoryManage.Application.Contracts.Containers.Dtos
{
    public class ContainerSearchDto : PagedAndSortedResultRequestDto
    {
        public string? ContainerCode { get; set; }
    }
}

