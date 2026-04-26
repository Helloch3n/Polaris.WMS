using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Validation;

namespace Polaris.WMS.Outbound.Application.Contracts.Waves;

public class WaveOrderDto : AuditedEntityDto<Guid>
{
    public string WaveNo { get; set; } = string.Empty;

    public WaveOrderStatus Status { get; set; }

    public string? Remark { get; set; }

    public List<WaveOrderLineDto> Lines { get; set; } = new();
}

public class WaveOrderLineDto : EntityDto<Guid>
{
    public Guid WaveOrderId { get; set; }
    public Guid SalesShipmentId { get; set; }
    public string SalesShipmentNo { get; set; } = string.Empty;
    public Guid SalesShipmentDetailId { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerCode { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal Qty { get; set; }
}

public class CreateWaveOrderDto : IValidationEnabled
{
    [Required]
    [MinLength(1)]
    public List<Guid> SalesShipmentIds { get; set; } = new();

    [MaxLength(1000)]
    public string? Remark { get; set; }
}

public class WaveOrderSearchDto : PagedAndSortedResultRequestDto
{
    public string? WaveNo { get; set; }
    public WaveOrderStatus? Status { get; set; }
}

