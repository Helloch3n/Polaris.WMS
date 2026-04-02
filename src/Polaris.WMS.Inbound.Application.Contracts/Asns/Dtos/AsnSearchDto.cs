using System.ComponentModel.DataAnnotations;
using Polaris.WMS.Asns;
using Volo.Abp.Application.Dtos;

namespace Polaris.WMS.Inbound.Application.Contracts.Asns.Dtos;

public class AsnSearchDto : PagedAndSortedResultRequestDto
{

    /// <summary>
    /// ASN 单号（精确查询）。
    /// </summary>
    [MaxLength(64)]
    public string? AsnNo { get; set; }

    /// <summary>
    /// 供应商名称（模糊查询）。
    /// </summary>
    [MaxLength(128)]
    public string? SupplierName { get; set; }

    public AsnStatus? Status { get; set; }

    /// <summary>
    /// 车牌号（子表明细模糊查询）。
    /// </summary>
    [MaxLength(64)]
    public string? LicensePlate { get; set; }
}