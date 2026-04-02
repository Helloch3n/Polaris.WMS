using System.ComponentModel.DataAnnotations;
using Polaris.WMS.PurchaseOrders;
using Volo.Abp.Application.Dtos;

namespace Polaris.WMS.Inbound.Application.Contracts.PurchaseOrders.Dtos;

public class PurchaseOrderSearchDto : PagedAndSortedResultRequestDto
{
    /// <summary>
    /// 采购单号（精确查询）。
    /// </summary>
    [MaxLength(64)]
    public string? PoNo { get; set; }

    /// <summary>
    /// 供应商名称（模糊查询）。
    /// </summary>
    [MaxLength(128)]
    public string? SupplierName { get; set; }


    public PurchaseOrderStatus? Status { get; set; }
}