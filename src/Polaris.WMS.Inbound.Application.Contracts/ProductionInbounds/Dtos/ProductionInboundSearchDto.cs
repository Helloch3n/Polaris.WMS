using System.ComponentModel.DataAnnotations;
using Polaris.WMS.ProductionInbounds;
using Volo.Abp.Application.Dtos;

namespace Polaris.WMS.Inbound.Application.Contracts.ProductionInbounds.Dtos
{
    /// <summary>
    /// 生产入库单分页查询参数
    /// </summary>
    public class ProductionInboundSearchDto : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// 入库单号（精确查询）。
        /// </summary>
        [MaxLength(64)]
        public string? OrderNo { get; set; }

        /// <summary>
        /// 来源单号（精确查询）。
        /// </summary>
        [MaxLength(64)]
        public string? SourceOrderNo { get; set; }


        public ProductionInboundStatus? Status { get; set; }
    }
}