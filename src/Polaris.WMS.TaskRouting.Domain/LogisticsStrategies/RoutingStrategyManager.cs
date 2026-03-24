using Polaris.WMS.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Polaris.WMS.TaskRouting.Domain.LogisticsStrategies;

public class RoutingStrategyManager(
    IRepository<RoutingStrategy, Guid> repository) : DomainService
{
    public async Task<RoutingStrategy> CreateAsync(string ruleName, int priority, MoveTaskType taskType, bool isActive,
        Guid? productCategoryId, Guid? productId, Guid sourceZoneId, Guid targetZoneId)
    {
        var route = new RoutingStrategy(ruleName, priority, taskType, isActive, productCategoryId, productId,
            sourceZoneId, targetZoneId);
        await repository.InsertAsync(route);
        return route;
    }

    public void CreateAsync()
    {
        throw new NotImplementedException();
    }
}