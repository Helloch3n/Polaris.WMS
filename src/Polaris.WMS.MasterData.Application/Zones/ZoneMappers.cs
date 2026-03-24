using Polaris.WMS.MasterData.Application.Contracts.Zones.Dtos;
using Polaris.WMS.MasterData.Domain.Zones;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace Polaris.WMS.MasterData.Application.Zones;

[Mapper]
public partial class ZoneMappers : MapperBase<Zone, ZoneDto>
{
    public override partial ZoneDto Map(Zone source);
    public override partial void Map(Zone source, ZoneDto destination);
}