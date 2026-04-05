using System.ComponentModel.DataAnnotations;
using Volo.Abp.Validation;

namespace Polaris.WMS.Inbound.Application.Contracts.PurchaseReceipts.Dtos;

/// <summary>
/// 创建采购收货单输入。
/// </summary>
public class CreatePurchaseReceiptDto : IValidationEnabled
{
    /// <summary>
    /// 来源单据类型（例如 ASN/PO）。
    /// </summary>
    [Required]
    [MaxLength(32)]
    public string SourceDocType { get; set; } = string.Empty;

    /// <summary>
    /// 来源单据号。
    /// </summary>
    [Required]
    [MaxLength(64)]
    public string SourceDocNo { get; set; } = string.Empty;

    /// <summary>
    /// 供应商 Id（可空）。
    /// </summary>
    public Guid? SupplierId { get; set; }

    /// <summary>
    /// 供应商名称快照。
    /// </summary>
    [MaxLength(256)]
    public string? SupplierName { get; set; }

    /// <summary>
    /// 备注。
    /// </summary>
    [MaxLength(1000)]
    public string? Remark { get; set; }

    /// <summary>
    /// 明细集合。
    /// </summary>
    [Required]
    [MinLength(1)]
    public List<CreatePurchaseReceiptDetailDto> Details { get; set; } = new();
}

/// <summary>
/// 创建采购收货单明细输入。
/// </summary>
public class CreatePurchaseReceiptDetailDto : IValidationEnabled
{
    /// <summary>
    /// 来源明细 Id（可空）。
    /// </summary>
    public Guid? SourceDetailId { get; set; }

    /// <summary>
    /// 物料 Id。
    /// </summary>
    [Required]
    public Guid ProductId { get; set; }

    /// <summary>
    /// 物料名称。
    /// </summary>
    [Required]
    [MaxLength(256)]
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// 物料编码。
    /// </summary>
    [Required]
    [MaxLength(64)]
    public string ProductCode { get; set; } = string.Empty;

    /// <summary>
    /// 接收数量。
    /// </summary>
    [Range(typeof(decimal), "0.0001", "79228162514264337593543950335")]
    public decimal ReceivedQuantity { get; set; }

    /// <summary>
    /// 容器 Id。
    /// </summary>
    [Required]
    public Guid ContainerId { get; set; }

    /// <summary>
    /// 容器编码。
    /// </summary>
    [Required]
    [MaxLength(64)]
    public string ContainerCode { get; set; } = string.Empty;

    /// <summary>
    /// 库位 Id。
    /// </summary>
    [Required]
    public Guid LocationId { get; set; }

    /// <summary>
    /// 库位编码快照（当前按 Guid 存储）。
    /// </summary>
    [Required]
    public Guid LocationCode { get; set; }

    /// <summary>
    /// 批次号。
    /// </summary>
    [MaxLength(64)]
    public string? BatchNo { get; set; }
}

