using System.ComponentModel.DataAnnotations;
using Volo.Abp.Validation;

namespace Polaris.WMS.Inbound.Application.Contracts.PurchaseReceipts.Dtos;

/// <summary>
/// 向采购收货明细新增收货记录输入。
/// </summary>
public class AddPurchaseReceiptRecordsDto : IValidationEnabled
{
    /// <summary>
    /// 采购收货单 Id。
    /// </summary>
    [Required]
    public Guid PurchaseReceiptId { get; set; }

    /// <summary>
    /// 采购收货明细 Id。
    /// </summary>
    [Required]
    public Guid DetailId { get; set; }

    /// <summary>
    /// 收货记录集合。
    /// </summary>
    [Required]
    [MinLength(1)]
    public List<AddPurchaseReceiptRecordDto> Records { get; set; } = new();
}

/// <summary>
/// 采购收货记录输入。
/// </summary>
public class AddPurchaseReceiptRecordDto : IValidationEnabled
{
    /// <summary>
    /// 实收数量。
    /// </summary>
    [Range(typeof(decimal), "0.0001", "79228162514264337593543950335")]
    public decimal ReceivedQuantity { get; set; }

    /// <summary>
    /// 容器编码。
    /// </summary>
    [Required]
    [MaxLength(64)]
    public string ContainerCode { get; set; } = string.Empty;


    /// <summary>
    /// 库位编码。
    /// </summary>
    [Required]
    [MaxLength(64)]
    public string LocationCode { get; set; } = string.Empty;

    /// <summary>
    /// 批次号（可空）。
    /// </summary>
    [MaxLength(64)]
    public string? BatchNo { get; set; }

    /// <summary>
    /// 供应商批次号（可空）。
    /// </summary>
    [MaxLength(64)]
    public string? SupplierBatchNo { get; set; }
}

