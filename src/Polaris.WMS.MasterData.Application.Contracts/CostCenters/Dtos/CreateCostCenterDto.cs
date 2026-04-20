using System.ComponentModel.DataAnnotations;

namespace Polaris.WMS.MasterData.Application.Contracts.CostCenters.Dtos;

public class CreateCostCenterDto
{
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string DepartmentCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string DepartmentName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string CompanyCode { get; set; } = string.Empty;
}

