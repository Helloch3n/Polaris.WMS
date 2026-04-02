using Polaris.WMS.ProductionInbounds;
using Volo.Abp.Application.Dtos;

namespace Polaris.WMS.Inbound.Application.Contracts.ProductionInbounds.Dtos
{
    public class ProductionInboundDto : AuditedEntityDto<Guid>
    {
        public string OrderNo { get; set; }
        public string SourceOrderNo { get; set; }
        public ProductionInboundType InboundType { get; set; }

        public Guid SourceDepartmentId { get; set; }
        public string SourceDepartmentName { get; set; }
        public string SourceDepartmentCode { get; set; }

        public Guid TargetWarehouseId { get; set; }
        public string TargetWarehouseName { get; set; }
        public string TargetWarehouseCode { get; set; }

        public ProductionInboundStatus Status { get; set; }

        public List<ProductionInboundDetailDto>? Details { get; set; }
    }

    public class ProductionInboundDetailDto : AuditedEntityDto<Guid>
    {
        public Guid ProductionInboundId { get; set; }
        public Guid ProductId { get; set; }

        public string ProductCode { get; set; }

        public string ProductName { get; set; }

        public string BatchNo { get; set; }
        public Guid ReelId { get; set; }
        public Guid ReelCode { get; set; }
        public decimal Qty { get; set; }
        public string Unit { get; set; }
        public decimal Weight { get; set; }

        public string SN { get; set; }
        public int LayerIndex { get; set; }

        public string RelatedOrderNo { get; set; }
        public string RelatedOrderNoLineNo { get; set; }

        public Guid ActualLocationId { get; set; }
        public string ActualLocationCode { get; set; }
        public ProductionInboundDetailStatus Status { get; set; }

        public string CraftVersion { get; set; }
    }
}
