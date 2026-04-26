using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Validation;

namespace Polaris.WMS.Outbound.Application.Contracts.PickLists;

public class PickListDto : AuditedEntityDto<Guid>
{
    public string PickNo { get; set; } = string.Empty;
    public Guid WaveOrderId { get; set; }
    public Guid TargetLocationId { get; set; }
    public string TargetLocationCode { get; set; } = string.Empty;
    public PickListStatus Status { get; set; }
    public string? Remark { get; set; }
    public List<PickListLineDto> Lines { get; set; } = new();
}

public class PickListLineDto : EntityDto<Guid>
{
    public Guid PickListId { get; set; }
    public Guid SalesShipmentId { get; set; }
    public string SalesShipmentNo { get; set; } = string.Empty;
    public Guid SalesShipmentDetailId { get; set; }
    public Guid SalesShipmentAllocationId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal Qty { get; set; }
    public Guid ContainerId { get; set; }
    public string ContainerCode { get; set; } = string.Empty;
    public Guid SourceLocationId { get; set; }
    public string SourceLocationCode { get; set; } = string.Empty;
    public Guid TargetLocationId { get; set; }
    public string TargetLocationCode { get; set; } = string.Empty;
    public Guid? MoveTaskId { get; set; }
    public string? MoveTaskNo { get; set; }
    public Guid? ActualLocationId { get; set; }
    public string BatchNo { get; set; } = string.Empty;
    public string SN { get; set; } = string.Empty;
    public bool IsPicked { get; set; }
}

public class CreatePickListDto : IValidationEnabled
{
    [Required]
    public Guid WaveOrderId { get; set; }

    [Required]
    [MaxLength(64)]
    public string TargetLocationCode { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Remark { get; set; }
}

public class PickListSearchDto : PagedAndSortedResultRequestDto
{
    public string? PickNo { get; set; }
    public PickListStatus? Status { get; set; }
    public Guid? WaveOrderId { get; set; }
}

