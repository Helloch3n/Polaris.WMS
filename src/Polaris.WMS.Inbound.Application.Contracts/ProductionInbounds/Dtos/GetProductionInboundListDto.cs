using Polaris.WMS.ProductionInbounds;
using Volo.Abp.Application.Dtos;

namespace Polaris.WMS.Inound.Application.Contracts.ProductionInbounds.Dtos
{
    /// <summary>
    /// 生产入库单分页查询参数
    /// </summary>
    public class GetProductionInboundListDto : PagedAndSortedResultRequestDto
    {
        public string? Filter { get; set; } // 模糊搜索：入库单号、来源单号

        public ProductionInboundType? InboundType { get; set; }

        public ProductionInboundStatus? Status { get; set; }

        public Guid? SourceDepartmentId { get; set; }

        public Guid? TargetWarehouseId { get; set; }
    }
}
