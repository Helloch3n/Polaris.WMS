using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Validation;

namespace Polaris.WMS.Outbound.Application.Contracts.SalesShipments;

public class SalesShipmentDto : AuditedEntityDto<Guid>
{
    public string ShipmentNo { get; set; } = string.Empty;
    public Guid? SourceSalesOrderId { get; set; }
    public string? SourceSalesOrderNo { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerCode { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public OutboundOrderStatus Status { get; set; }
    public string? DeliveryContactName { get; set; }
    public string? DeliveryPhone { get; set; }
    public string? DeliveryAddress { get; set; }
    public string? Remark { get; set; }
    public List<SalesShipmentDetailDto> Details { get; set; } = new();
}

public class SalesShipmentDetailDto : EntityDto<Guid>
{
    public Guid SalesShipmentId { get; set; }
    public Guid? SourceSalesOrderLineId { get; set; }
    public int LineNo { get; set; }
    public Guid ProductId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal RequiredQty { get; set; }
    public decimal AllocatedQty { get; set; }
    public decimal PickedQty { get; set; }
    public decimal ShippedQty { get; set; }
    public string? Remark { get; set; }
    public List<SalesShipmentAllocationDto> Allocations { get; set; } = new();
    public List<SalesShipmentRecordDto> Records { get; set; } = new();
}

public class SalesShipmentAllocationDto : AuditedEntityDto<Guid>
{
    public Guid SalesShipmentId { get; set; }
    public Guid SalesShipmentDetailId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal Qty { get; set; }
    public Guid ContainerId { get; set; }
    public string ContainerCode { get; set; } = string.Empty;
    public Guid SourceLocationId { get; set; }
    public string SourceLocationCode { get; set; } = string.Empty;
    public string BatchNo { get; set; } = string.Empty;
    public string SN { get; set; } = string.Empty;
    public SalesShipmentAllocationStatus Status { get; set; }
}

public class SalesShipmentRecordDto : EntityDto<Guid>
{
    public Guid SalesShipmentId { get; set; }
    public Guid SalesShipmentDetailId { get; set; }
    public Guid? SourceSalesOrderLineId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal Qty { get; set; }
    public Guid ContainerId { get; set; }
    public string ContainerCode { get; set; } = string.Empty;
    public Guid LocationId { get; set; }
    public string LocationCode { get; set; } = string.Empty;
    public string BatchNo { get; set; } = string.Empty;
    public string SN { get; set; } = string.Empty;
}

public class CreateSalesShipmentDto : IValidationEnabled
{
    [Required]
    [MaxLength(64)]
    public string ShipmentNo { get; set; } = string.Empty;

    public Guid? SourceSalesOrderId { get; set; }

    [MaxLength(64)]
    public string? SourceSalesOrderNo { get; set; }

    [Required]
    public Guid CustomerId { get; set; }

    [MaxLength(50)]
    public string? DeliveryContactName { get; set; }

    [MaxLength(30)]
    public string? DeliveryPhone { get; set; }

    [MaxLength(500)]
    public string? DeliveryAddress { get; set; }

    [MaxLength(1000)]
    public string? Remark { get; set; }

    [Required]
    [MinLength(1)]
    public List<CreateSalesShipmentDetailDto> Details { get; set; } = new();
}

public class UpdateSalesShipmentDto : IValidationEnabled
{
    [Required]
    public Guid CustomerId { get; set; }

    [MaxLength(50)]
    public string? DeliveryContactName { get; set; }

    [MaxLength(30)]
    public string? DeliveryPhone { get; set; }

    [MaxLength(500)]
    public string? DeliveryAddress { get; set; }

    [MaxLength(1000)]
    public string? Remark { get; set; }

    [Required]
    [MinLength(1)]
    public List<CreateSalesShipmentDetailDto> Details { get; set; } = new();
}

public class CreateSalesShipmentDetailDto : IValidationEnabled
{
    public Guid? SourceSalesOrderLineId { get; set; }
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
    public decimal RequiredQty { get; set; }

    [MaxLength(1000)]
    public string? Remark { get; set; }
}

public class AddSalesShipmentRecordsDto : IValidationEnabled
{
    [Required]
    public Guid SalesShipmentId { get; set; }

    [Required]
    public Guid DetailId { get; set; }

    [Required]
    [MinLength(1)]
    public List<AddSalesShipmentRecordDto> Records { get; set; } = new();
}

public class CreateSalesShipmentAllocationDto : IValidationEnabled
{
    [Required]
    public Guid SalesShipmentId { get; set; }

    [Required]
    public Guid DetailId { get; set; }

    [Range(typeof(decimal), "0.0001", "79228162514264337593543950335")]
    public decimal Qty { get; set; }

    [Required]
    [MaxLength(64)]
    public string ContainerCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(64)]
    public string LocationCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string BatchNo { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string SN { get; set; } = string.Empty;
}

public class MarkSalesShipmentAllocationPickedDto : IValidationEnabled
{
    [Required]
    public Guid SalesShipmentId { get; set; }

    [Required]
    public Guid DetailId { get; set; }

    [Required]
    public Guid AllocationId { get; set; }
}

public class AddSalesShipmentRecordDto : IValidationEnabled
{
    [Range(typeof(decimal), "0.0001", "79228162514264337593543950335")]
    public decimal Qty { get; set; }

    [Required]
    [MaxLength(64)]
    public string ContainerCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(64)]
    public string LocationCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string BatchNo { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string SN { get; set; } = string.Empty;
}

public class SalesShipmentSearchDto : PagedAndSortedResultRequestDto
{
    public string? ShipmentNo { get; set; }
    public string? CustomerCode { get; set; }
    public OutboundOrderStatus? Status { get; set; }
}


