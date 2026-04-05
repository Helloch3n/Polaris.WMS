using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Polaris.WMS.Inbound.Application.Contracts.PurchaseReceipts.Dtos;

/// <summary>
/// 采购收货单分页查询参数。
/// </summary>
public class PurchaseReceiptSearchDto : PagedAndSortedResultRequestDto
{
    /// <summary>
    /// 收货单号（精确查询）。
    /// </summary>
    [MaxLength(64)]
    public string? ReceiptNo { get; set; }

    /// <summary>
    /// 来源单据号（精确查询）。
    /// </summary>
    [MaxLength(64)]
    public string? SourceDocNo { get; set; }

    /// <summary>
    /// 来源单据类型（精确查询）。
    /// </summary>
    [MaxLength(32)]
    public string? SourceDocType { get; set; }
}

