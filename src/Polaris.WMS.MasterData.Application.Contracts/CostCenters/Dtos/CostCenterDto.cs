using Volo.Abp.Application.Dtos;

namespace Polaris.WMS.MasterData.Application.Contracts.CostCenters.Dtos;

public class CostCenterDto : AuditedEntityDto<Guid>
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string DepartmentCode { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public string CompanyCode { get; set; } = string.Empty;
}

