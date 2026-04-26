using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Polaris.WMS.MasterData.Application.Contracts.Customers;

public class CustomerDto : AuditedEntityDto<Guid>
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ContactName { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public bool IsEnabled { get; set; }
    public string? Remark { get; set; }
}

public class CreateUpdateCustomerDto
{
    [Required]
    [StringLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(50)]
    public string? ContactName { get; set; }

    [StringLength(30)]
    public string? Phone { get; set; }

    [StringLength(500)]
    public string? Address { get; set; }

    public bool IsEnabled { get; set; } = true;

    [StringLength(1000)]
    public string? Remark { get; set; }
}

public class CustomerSearchDto : PagedAndSortedResultRequestDto
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public bool? IsEnabled { get; set; }
}

