using Polaris.WMS.Inbound.Application.Contracts.Asns.Dtos;
using Polaris.WMS.Inbound.Domain.Asns;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace Polaris.WMS.Inbound.Application.Asns;

[Mapper]
public partial class AsnMapper : MapperBase<AdvancedShippingNotice, AdvancedShippingNoticeDto>
{
    public override partial AdvancedShippingNoticeDto Map(AdvancedShippingNotice source);
    public override partial void Map(AdvancedShippingNotice source, AdvancedShippingNoticeDto destination);
}