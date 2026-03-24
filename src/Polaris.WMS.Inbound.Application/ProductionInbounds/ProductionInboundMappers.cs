using Polaris.WMS.Inbound.Domain.ProductionInbounds;
using Polaris.WMS.Inound.Application.Contracts.ProductionInbounds.Dtos;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace Polaris.WMS.Inbound.Application.ProductionInbounds
{
    [Mapper]
    public partial class ProductionInboundMappers : MapperBase<ProductionInbound, ProductionInboundDto>
    {
        // 因为我们的 DTO 字段名和 Entity 字段名目前是完全一致的，
        // 所以不需要写 [MapProperty]，Mapperly 会自动按照同名原则进行极速映射。

        public override partial ProductionInboundDto Map(ProductionInbound source);

        public override partial void Map(ProductionInbound source, ProductionInboundDto destination);
    }
}
