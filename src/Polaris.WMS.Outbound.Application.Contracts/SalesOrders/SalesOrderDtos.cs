using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Validation;

namespace Polaris.WMS.Outbound.Application.Contracts.SalesOrders;

public class SalesOrderDto : AuditedEntityDto<Guid>
{
    public string OrderNo { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public string CustomerCode { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public SalesOrderStatus Status { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? ExpectedDeliveryTime { get; set; }
    public string? Remark { get; set; }
    public List<SalesOrderDetailDto> Details { get; set; } = new();
}

public class SalesOrderDetailDto : EntityDto<Guid>
{
    public Guid SalesOrderId { get; set; }
    public int LineNo { get; set; }
    public Guid ProductId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal Qty { get; set; }
    public decimal AllocatedQty { get; set; }
    public decimal ShippedQty { get; set; }
    public string? Remark { get; set; }
}

public class CreateSalesOrderDto : IValidationEnabled
{
    [Required]
    [MaxLength(64)]
    public string OrderNo { get; set; } = string.Empty;

    [Required]
    public Guid CustomerId { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.Now;
    public DateTime? ExpectedDeliveryTime { get; set; }

    [MaxLength(1000)]
    public string? Remark { get; set; }

    [Required]
    [MinLength(1)]
    public List<CreateSalesOrderDetailDto> Details { get; set; } = new();
}

public class UpdateSalesOrderDto : IValidationEnabled
{
    [Required]
    public Guid CustomerId { get; set; }

    public DateTime OrderDate { get; set; }
    public DateTime? ExpectedDeliveryTime { get; set; }

    [MaxLength(1000)]
    public string? Remark { get; set; }

    [Required]
    [MinLength(1)]
    public List<CreateSalesOrderDetailDto> Details { get; set; } = new();
}

public class CreateSalesOrderDetailDto : IValidationEnabled
{
    public int LineNo { get; set; }

    [Required]
    public Guid ProductId { get; set; }

    [Required]
    [MaxLength(50)]
    public string ProductCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string ProductName { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Unit { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.0001", "79228162514264337593543950335")]
    public decimal Qty { get; set; }

    [MaxLength(1000)]
    public string? Remark { get; set; }
}

public class SalesOrderSearchDto : PagedAndSortedResultRequestDto
{
    public string? OrderNo { get; set; }
    public string? CustomerCode { get; set; }
    public SalesOrderStatus? Status { get; set; }
}

