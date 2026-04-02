using Polaris.WMS.Asns;
using Volo.Abp.Application.Dtos;

namespace Polaris.WMS.Inbound.Application.Contracts.Asns.Dtos;

public class GetAsnListDto : PagedAndSortedResultRequestDto
{
    public string Filter { get; set; }
    public AsnStatus? Status { get; set; }
    public string LicensePlate { get; set; } 
}