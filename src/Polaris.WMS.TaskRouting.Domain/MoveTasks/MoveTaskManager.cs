using Polaris.WMS.BillNumbers;
using Polaris.WMS.Tasks;
using Polaris.WMS.Tasks.MoveTask.Events;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Polaris.WMS.TaskRouting.Domain.MoveTasks;

public class MoveTaskManager(
    IBillNumberGenerator billNumberGenerator,
    IRepository<MoveTask, Guid> moveTaskRepository) : DomainService
{
    public async Task<MoveTask> CreateMoveTaskAsync(MoveTaskType taskType, Guid containerId, string containerCode,
        Guid sourceLocationId,
        Guid targetLocationId)
    {
        var orderNo = await billNumberGenerator.GetNextNumberAsync("MOV");
        var normalizedOrderNo = Check.NotNullOrWhiteSpace(orderNo, nameof(orderNo));

        var exists = await moveTaskRepository.AnyAsync(x => x.TaskNo == normalizedOrderNo);
        if (exists)
        {
            throw new BusinessException("搬运任务号已存在")
                .WithData("搬运任务号", normalizedOrderNo);
        }

        // 1. 防呆拦截（核心业务规则：一个托盘不能有两个活着的任务）
        var hasActiveTask = await moveTaskRepository.AnyAsync(x =>
            x.ContainerId == containerId &&
            x.Status != MoveTaskStatus.Completed &&
            x.Status != MoveTaskStatus.Cancelled);

        if (hasActiveTask)
        {
            throw new UserFriendlyException($"载具 {containerId} 已有正在执行的搬运任务，切勿重复派发！");
        }

        var moveTask = MoveTask.Create(Guid.NewGuid(), orderNo, taskType, containerId, containerCode, sourceLocationId,
            targetLocationId);

        await moveTaskRepository.InsertAsync(moveTask);

        moveTask.GenerateCreatedEvent();

        return moveTask;
    }
}