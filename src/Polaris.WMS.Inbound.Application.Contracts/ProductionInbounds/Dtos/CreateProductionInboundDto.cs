using System.ComponentModel.DataAnnotations;
using Polaris.WMS.ProductionInbounds;

namespace Polaris.WMS.Inound.Application.Contracts.ProductionInbounds.Dtos
{
    // <summary>
    /// 创建生产入库单头表 DTO
    /// </summary>
    public class CreateProductionInboundDto
    {
        [Required(ErrorMessage = "来源单号不能为空")]
        [MaxLength(64)]
        public string SourceOrderNo { get; set; }

        [Required(ErrorMessage = "请选择生产入库类型")]
        public ProductionInboundType InboundType { get; set; }

        [Required(ErrorMessage = "来源车间不能为空")]
        public Guid SourceDepartmentId { get; set; }

        [Required(ErrorMessage = "目标入库仓库不能为空")]
        public Guid TargetWarehouseId { get; set; }

        public List<CreateProductionInboundDetailDto>? Details { get; set; }
    }

    public class CreateProductionInboundDetailDto
    {
        public Guid ProductId { get; set; }
        public string BatchNo { get; set; }
        public Guid ReelId { get; set; }
        public decimal Qty { get; set; }
        public string Unit { get; set; }
        public decimal Weight { get; set; } 
        public string SN { get; set; }
        public int LayerIndex { get; set; }
        public string RelatedOrderNo { get; set; }
        public string RelatedOrderNoLineNo { get; set; }
        public Guid ActualLocationId { get; set; }
        public string CraftVersion { get; set; }
    }
}
