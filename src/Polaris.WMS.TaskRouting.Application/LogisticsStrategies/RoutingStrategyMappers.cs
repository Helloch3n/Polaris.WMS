using Polaris.WMS.TaskRouting.Application.Contracts.LogisticsStrategies.Dtos;
using Polaris.WMS.TaskRouting.Domain.LogisticsStrategies;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace Polaris.WMS.TaskRouting.Application.LogisticsStrategies;

[Mapper]
public partial class RoutingStrategyMappers : MapperBase<RoutingStrategy, RoutingStrategyDto>
{
    public override partial RoutingStrategyDto Map(RoutingStrategy source);
    public override partial void Map(RoutingStrategy source, RoutingStrategyDto destination);
}