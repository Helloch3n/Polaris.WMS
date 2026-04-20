using Volo.Abp.Application.Dtos;

namespace Polaris.WMS.MasterData.Application.Contracts.CostCenters.Dtos;

public class CostCenterSearchDto : PagedAndSortedResultRequestDto
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? DepartmentCode { get; set; }
    public string? DepartmentName { get; set; }
    public string? CompanyCode { get; set; }
}

