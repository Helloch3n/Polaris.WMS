using Polaris.WMS.Inbound.Application.Contracts.MiscInboundOrders.Dtos;
using Polaris.WMS.Inbound.Domain.MiscOrders;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace Polaris.WMS.Inbound.Application.MiscInboundOrders;

[Mapper]
public partial class MiscInboundOrderMappers : MapperBase<MiscInboundOrder, MiscInboundOrderDto>
{
    public override partial MiscInboundOrderDto Map(MiscInboundOrder source);
    public override partial void Map(MiscInboundOrder source, MiscInboundOrderDto destination);
}

