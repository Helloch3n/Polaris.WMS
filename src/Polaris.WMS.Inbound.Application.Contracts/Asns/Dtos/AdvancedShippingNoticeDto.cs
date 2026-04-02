using Polaris.WMS.Asns;
using Volo.Abp.Application.Dtos;

namespace Polaris.WMS.Inbound.Application.Contracts.Asns.Dtos;

public class AdvancedShippingNoticeDto: AuditedEntityDto<Guid>
{
    public string AsnNo { get; set; }
    public Guid SupplierId { get; set; }
    public string SupplierCode { get; set; }
    public string SupplierName { get; set; }
    public AsnStatus Status { get; set; }
    public DateTime? ExpectedArrivalTime { get; set; }
    
    public List<AsnDetailDto> Details { get; set; } = new();
}