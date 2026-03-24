using Polaris.WMS.TaskRouting.Domain.Integration.MasterData;
using Polaris.WMS.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace Polaris.WMS.TaskRouting.Domain.LogisticsStrategies;

public class TableDrivenZoneRoutingStrategy(
    IRepository<RoutingStrategy, Guid> ruleRepository,
    ILocationAllocationAdapter locationAdapter)
    : IZoneRoutingStrategy, ITransientDependency
{
    public async Task<Guid> CalculateTargetZoneAsync(MoveTaskType taskType, Guid currentLocationId)
    {
        //var product = await productRepository.GetAsync(productId);


        var activeRules = await ruleRepository.GetListAsync(x => x.IsActive && x.TaskType == taskType);
        if (activeRules == null || !activeRules.Any())
        {
            throw new UserFriendlyException($"未找到适用的物流路由规则！");
        }

        activeRules.OrderBy(x => x.Priority);
        var zoneId = await locationAdapter.GetZoneIdByLocationIdAsync(currentLocationId);

        foreach (var rule in activeRules)
        {
            var matchSource = !rule.SourceZoneId.HasValue || rule.SourceZoneId == zoneId;
            //bool matchCategory = !rule.ProductCategoryId.HasValue || rule.ProductCategoryId == product.CategoryId;
            //bool matchProduct = !rule.ProductId.HasValue || rule.ProductId == productId;
            if (matchSource) return rule.TargetZoneId;
        }

        throw new UserFriendlyException($"未找到适用的物流路由规则！");
    }
}