using Volo.Abp.Application.Dtos;

namespace Polaris.WMS.Inbound.Application.Contracts.Asns.Dtos;

public class AsnDetailDto : EntityDto<Guid>
{
    public Guid AsnId { get; set; }
    public string ScmAsnRowNo { get; set; }
    public string SourcePoNo { get; set; }
    public int SourcePoLineNo { get; set; }
    public Guid ProductId { get; set; }
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public string SupplierBatchNo { get; set; }
    public string LicensePlate { get; set; }
    public string UoM { get; set; }
    public decimal ExpectedQty { get; set; }
    public decimal ReceivedQty { get; set; }
}