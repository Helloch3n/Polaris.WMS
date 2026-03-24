using Polaris.WMS.TaskRouting.Application.Contracts.LogisticsStrategies;
using Polaris.WMS.TaskRouting.Application.Contracts.LogisticsStrategies.Dtos;
using Polaris.WMS.TaskRouting.Domain.LogisticsStrategies;
using Volo.Abp.Application.Services;

namespace Polaris.WMS.TaskRouting.Application.LogisticsStrategies;

public class RoutingStrategyAppService(
    RoutingStrategyManager routingStrategyManager
) : ApplicationService, IRoutingStrategyAppService
{
    /// <summary>
    /// 创建路由策略
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<RoutingStrategyDto> CreateAsync(CreateRoutingStrategyDto input)
    {
        var route = await routingStrategyManager.CreateAsync(
            input.RuleName,
            input.Priority,
            input.TaskType,
            input.IsActive,
            input.ProductCategoryId,
            input.ProductId,
            input.SourceZoneId,
            input.TargetZoneId
        );

        return ObjectMapper.Map<RoutingStrategy, RoutingStrategyDto>(route);
    }
}