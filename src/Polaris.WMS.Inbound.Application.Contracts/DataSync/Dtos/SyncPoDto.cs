using System.ComponentModel.DataAnnotations;

namespace Polaris.WMS.Inbound.Application.Contracts.DataSync.Dtos;

public class SyncPoDto
{
    [Required] public string PoNo { get; set; }
    [Required] public string SupplierCode { get; set; }
    public string SupplierName { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }

    public List<SyncPoDetailDto> Details { get; set; } = new();
}

public class SyncPoDetailDto
{
    [Required] public int LineNo { get; set; }
    [Required] public string ProductCode { get; set; }
    public string ProductName { get; set; }
    [Required] public string UoM { get; set; }

    [Range(0.0001, double.MaxValue, ErrorMessage = "期望数量必须大于0")]
    public decimal ExpectedQty { get; set; }

    public bool IsQualityCheckRequired { get; set; } = true;
}