using Polaris.WMS.TaskRouting.Application.Contracts.Integration.PickTasks;
using Polaris.WMS.TaskRouting.Domain.MoveTasks;
using Volo.Abp.Application.Services;

namespace Polaris.WMS.TaskRouting.Application.Integration.PickTasks;

public class TaskRoutingIntegrationService(
    MoveTaskManager moveTaskManager)
    : ApplicationService, ITaskRoutingIntegrationService
{
    /// <inheritdoc />
    public async Task<PickTaskIntegrationDto> CreatePickDownTaskAsync(CreatePickDownTaskInput input)
    {
        var task = await moveTaskManager.CreateMoveTaskAsync(
            input.TaskType,
            input.ContainerId,
            input.ContainerCode,
            input.SourceLocationId,
            input.SourceLocationCode,
            input.TargetLocationId,
            input.TargetLocationCode);

        return new PickTaskIntegrationDto
        {
            TaskId = task.Id,
            TaskNo = task.TaskNo
        };
    }
}

