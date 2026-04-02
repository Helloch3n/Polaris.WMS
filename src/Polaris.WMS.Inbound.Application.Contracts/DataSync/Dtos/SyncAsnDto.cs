using System.ComponentModel.DataAnnotations;

namespace Polaris.WMS.Inbound.Application.Contracts.DataSync.Dtos;

public class SyncAsnDto
{
    [Required] public string AsnNo { get; set; }
    [Required] public string SupplierCode { get; set; }
    public string SupplierName { get; set; }
    public string LicensePlate { get; set; }
    public DateTime? ExpectedArrivalTime { get; set; }

    public List<SyncAsnDetailDto> Details { get; set; } = new();
}

public class SyncAsnDetailDto
{
    [Required] public string ScmAsnRowNo { get; set; } // SCM 传来的行号/批次号
    [Required] public string SourcePoNo { get; set; }
    [Required] public int SourcePoLineNo { get; set; }
    [Required] public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public string UoM { get; set; }
    [Range(0.0001, double.MaxValue)] public decimal ExpectedQty { get; set; }
    public string SupplierBatchNo { get; set; }
}