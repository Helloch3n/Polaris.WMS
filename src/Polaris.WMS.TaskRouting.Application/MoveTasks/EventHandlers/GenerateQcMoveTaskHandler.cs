using Polaris.WMS.Inventories.Invnentory.Events;
using Polaris.WMS.TaskRouting.Application.Contracts.MoveTasks;
using Polaris.WMS.TaskRouting.Application.Contracts.MoveTasks.Dtos;
using Polaris.WMS.TaskRouting.Domain.LogisticsStrategies;
using Polaris.WMS.TaskRouting.Domain.MoveTasks;
using Polaris.WMS.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Uow;

namespace Polaris.WMS.TaskRouting.Application.MoveTasks.EventHandlers;

public class GenerateQcMoveTaskHandler(
    IMoveTaskAppService moveTaskAppService,
    IZoneRoutingStrategy zoneRoutingStrategy,
    ILocationAllocationStrategy allocationStrategy,
    MoveTaskManager moveTaskManager)
    : ILocalEventHandler<HoldInventoryCreatedEto>, ITransientDependency
{
    public async Task HandleEventAsync(HoldInventoryCreatedEto eventData)
    {
        // 1. 宏观计算：找大区
        Guid targetZoneId = await zoneRoutingStrategy.CalculateTargetZoneAsync(
            MoveTaskType.MoveToQc,
            eventData.CurrentLocationId // 此属性需在事件载体中补充
        );
        if (targetZoneId == Guid.Empty)
        {
            return;
        }

        // 2. 微观计算：找格子
        var targetLocationInfo = await allocationStrategy.AllocateLocationAsync(
            targetZoneId
        );

        // 生成搬运任务！
        await moveTaskManager.CreateMoveTaskAsync(
            MoveTaskType.MoveToQc,
            eventData.ContainerId, // Eto 里的属性
            eventData.ContainerCode, // Eto 里的属性
            eventData.CurrentLocationId,
            eventData.CurrentLocationCode,
            targetLocationInfo.Id,
            targetLocationInfo.Code);
    }
}