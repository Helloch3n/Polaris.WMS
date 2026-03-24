using Polaris.WMS.TaskRouting.Application.Contracts.LogisticsStrategies.Dtos;
using Volo.Abp.Application.Services;

namespace Polaris.WMS.TaskRouting.Application.Contracts.LogisticsStrategies;

public interface IRoutingStrategyAppService : IApplicationService
{
    Task<RoutingStrategyDto> CreateAsync(CreateRoutingStrategyDto input);
}