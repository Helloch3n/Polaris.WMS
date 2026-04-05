using System.ComponentModel.DataAnnotations;
using Polaris.WMS.Inbound;

namespace Polaris.WMS.Inbound.Application.Contracts.PurchaseReceipts.Dtos;

/// <summary>
/// 更新采购收货明细 ERP 同步状态输入。
/// </summary>
public class ChangePurchaseReceiptDetailErpSyncStatusDto
{
    /// <summary>
    /// 采购收货单 Id。
    /// </summary>
    [Required]
    public Guid PurchaseReceiptId { get; set; }

    /// <summary>
    /// 收货明细 Id。
    /// </summary>
    [Required]
    public Guid DetailId { get; set; }

    /// <summary>
    /// 目标 ERP 同步状态。
    /// </summary>
    [Required]
    public PurchaseReceiptErpSyncStatus NewStatus { get; set; }

    /// <summary>
    /// 失败时的错误信息。
    /// </summary>
    [MaxLength(1000)]
    public string? ErrorMessage { get; set; }
}


