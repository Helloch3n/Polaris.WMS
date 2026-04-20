using Polaris.WMS.Outbound.Application.Contracts.MiscOutboundOrders.Dtos;
using Polaris.WMS.Outbound.Domain.MiscOrders;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace Polaris.WMS.Outbound.Application.MiscOutboundOrders;

[Mapper]
public partial class MiscOutboundOrderMappers : MapperBase<MiscOutboundOrder, MiscOutboundOrderDto>
{
    public override partial MiscOutboundOrderDto Map(MiscOutboundOrder source);
    public override partial void Map(MiscOutboundOrder source, MiscOutboundOrderDto destination);
}

