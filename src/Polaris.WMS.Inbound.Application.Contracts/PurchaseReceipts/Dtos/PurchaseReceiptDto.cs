using Volo.Abp.Application.Dtos;
using Polaris.WMS.Inbound;

namespace Polaris.WMS.Inbound.Application.Contracts.PurchaseReceipts.Dtos;

/// <summary>
/// 采购收货单 DTO。
/// </summary>
public class PurchaseReceiptDto : AuditedEntityDto<Guid>
{
    public string ReceiptNo { get; set; } = string.Empty;
    public string SourceDocType { get; set; } = string.Empty;
    public string SourceDocNo { get; set; } = string.Empty;
    public Guid? SupplierId { get; set; }
    public string? SupplierName { get; set; }
    public string? Remark { get; set; }
    public List<PurchaseReceiptDetailDto> Details { get; set; } = new();
}

/// <summary>
/// 采购收货单明细 DTO。
/// </summary>
public class PurchaseReceiptDetailDto : EntityDto<Guid>
{
    public Guid PurchaseReceiptId { get; set; }
    public Guid? SourceAsnLineId { get; set; }
    public Guid? SourcePoLineId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductCode { get; set; } = string.Empty;
    public decimal ExpectedQuantity { get; set; }
    public decimal ReceivedQuantity { get; set; }
    public string? BatchNo { get; set; }
    public PurchaseReceiptErpSyncStatus ErpSyncStatus { get; set; }
    public string? ErpSyncErrorMessage { get; set; }
    public List<PurchaseRecordDto> Records { get; set; } = new();
}

/// <summary>
/// 采购收货记录 DTO。
/// </summary>
public class PurchaseRecordDto : EntityDto<Guid>
{
    public Guid PurchaseReceiptId { get; set; }
    public Guid PurchaseReceiptDetailId { get; set; }
    public Guid? SourceDetailId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductCode { get; set; } = string.Empty;
    public decimal ReceivedQuantity { get; set; }
    public Guid ContainerId { get; set; }
    public string ContainerCode { get; set; } = string.Empty;
    public Guid LocationId { get; set; }
    public string LocationCode { get; set; } = string.Empty;
    public string? BatchNo { get; set; }
    public string? SupplierBatchNo { get; set; }
}


