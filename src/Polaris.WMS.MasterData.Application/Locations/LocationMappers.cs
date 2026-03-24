using Polaris.WMS.MasterData.Application.Contracts.Locations.Dtos;
using Polaris.WMS.MasterData.Domain.Locations;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace Polaris.WMS.MasterData.Application.Locations;

[Mapper]
public partial class LocationMappers : MapperBase<Location, LocationDto>
{
    public override partial LocationDto Map(Location source);
    public override partial void Map(Location source, LocationDto destination);
}