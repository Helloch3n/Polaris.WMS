using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Validation;

namespace Polaris.WMS.Outbound.Application.Contracts.Handovers;

public class OutboundHandoverOrderDto : AuditedEntityDto<Guid>
{
    public string HandoverNo { get; set; } = string.Empty;
    public Guid OutboundReviewOrderId { get; set; }
    public OutboundHandoverStatus Status { get; set; }
    public string? Remark { get; set; }
    public List<OutboundHandoverLineDto> Lines { get; set; } = new();
}

public class OutboundHandoverLineDto : EntityDto<Guid>
{
    public Guid OutboundHandoverOrderId { get; set; }
    public Guid ReviewLineId { get; set; }
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

public class CreateOutboundHandoverOrderDto : IValidationEnabled
{
    [Required]
    public Guid OutboundReviewOrderId { get; set; }

    [MaxLength(1000)]
    public string? Remark { get; set; }
}

public class OutboundHandoverOrderSearchDto : PagedAndSortedResultRequestDto
{
    public string? HandoverNo { get; set; }
    public OutboundHandoverStatus? Status { get; set; }
    public Guid? OutboundReviewOrderId { get; set; }
}

