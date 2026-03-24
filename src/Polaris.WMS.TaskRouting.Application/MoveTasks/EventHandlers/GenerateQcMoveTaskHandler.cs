using Polaris.WMS.Inventorys.Events;
using Polaris.WMS.TaskRouting.Application.Contracts.MoveTasks;
using Polaris.WMS.TaskRouting.Application.Contracts.MoveTasks.Dtos;
using Polaris.WMS.TaskRouting.Domain.LogisticsStrategies;
using Polaris.WMS.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;

namespace Polaris.WMS.TaskRouting.Application.MoveTasks.EventHandlers;

public class GenerateQcMoveTaskHandler(
    IMoveTaskAppService moveTaskAppService,
    IZoneRoutingStrategy zoneRoutingStrategy,
    ILocationAllocationStrategy allocationStrategy)
    : ILocalEventHandler<HoldInventoryCreatedEvent>, ITransientDependency
{
    public async Task HandleEventAsync(HoldInventoryCreatedEvent eventData)
    {
        // 1. 宏观计算：找大区
        Guid targetZoneId = await zoneRoutingStrategy.CalculateTargetZoneAsync(
            MoveTaskType.MoveToQc,
            eventData.CurrentLocationId // 此属性需在事件载体中补充
        );

        // 2. 微观计算：找格子
        Guid targetLocationId = await allocationStrategy.AllocateLocationAsync(
            targetZoneId
        );

        // 生成搬运任务！
        await moveTaskAppService.CreateAsync(new CreateMoveTaskDto
        {
            ContainerId = eventData.ContainerId,
            FromLocationId = eventData.CurrentLocationId,
            TargetLocationId = targetLocationId,
            TaskType = MoveTaskType.MoveToQc // 明确这是一个送检跑腿任务
        });
    }
}