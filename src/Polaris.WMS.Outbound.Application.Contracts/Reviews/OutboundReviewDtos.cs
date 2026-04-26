using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Validation;

namespace Polaris.WMS.Outbound.Application.Contracts.Reviews;

public class OutboundReviewOrderDto : AuditedEntityDto<Guid>
{
    public string ReviewNo { get; set; } = string.Empty;
    public Guid PickListId { get; set; }
    public OutboundReviewStatus Status { get; set; }
    public string? Remark { get; set; }
    public List<OutboundReviewLineDto> Lines { get; set; } = new();
}

public class OutboundReviewLineDto : EntityDto<Guid>
{
    public Guid OutboundReviewOrderId { get; set; }
    public Guid PickListLineId { get; set; }
    public Guid SalesShipmentId { get; set; }
    public Guid SalesShipmentDetailId { get; set; }
    public Guid SalesShipmentAllocationId { get; set; }
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

public class CreateOutboundReviewOrderDto : IValidationEnabled
{
    [Required]
    public Guid PickListId { get; set; }

    [MaxLength(1000)]
    public string? Remark { get; set; }
}

public class OutboundReviewOrderSearchDto : PagedAndSortedResultRequestDto
{
    public string? ReviewNo { get; set; }
    public OutboundReviewStatus? Status { get; set; }
    public Guid? PickListId { get; set; }
}

