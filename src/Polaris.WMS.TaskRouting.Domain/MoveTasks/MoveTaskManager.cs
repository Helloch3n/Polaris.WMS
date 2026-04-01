using Polaris.WMS.BillNumbers;
using Polaris.WMS.TaskRouting.Domain.Integration.MasterData;
using Polaris.WMS.Tasks;
using Polaris.WMS.Tasks.MoveTask.Events;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Polaris.WMS.TaskRouting.Domain.MoveTasks;

public class MoveTaskManager(
    IBillNumberGenerator billNumberGenerator,
    IRepository<MoveTask, Guid> moveTaskRepository,
    IExternalLocationProvider externalLocationProvider) : DomainService
{
    public async Task<MoveTask> CreateMoveTaskAsync(MoveTaskType taskType, Guid containerId, string containerCode,
        Guid sourceLocationId,
        string sourceLocationCode,
        Guid targetLocationId,
        string targetLocationCode)
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
            sourceLocationCode,
            targetLocationId, targetLocationCode);

        await moveTaskRepository.InsertAsync(moveTask);

        moveTask.GenerateCreatedEvent();

        return moveTask;
    }

    public async Task<MoveTask> CompleteMoveTaskAsync(Guid taskId, string actualLocationCode)
    {
        // 1. 获取任务
        var task = await moveTaskRepository.GetAsync(taskId);

        // 2. 状态校验（领域规则）
        if (task.Status == MoveTaskStatus.Cancelled || task.Status == MoveTaskStatus.Completed)
        {
            throw new UserFriendlyException($"任务 {task.TaskNo} 的状态为 {task.Status}，不允许执行完成操作！");
        }

        var locationInfo = await externalLocationProvider.GetByCodeAsync(actualLocationCode);
        if (locationInfo == null)
        {
            throw new UserFriendlyException($"系统不存在编码为 '{actualLocationCode}' 的库位，请检查扫描是否有误！");
        }

        // 3. 调用实体内部方法，改变状态并触发事件
        task.Complete(locationInfo.Id);

        // 4. 更新到数据库
        await moveTaskRepository.UpdateAsync(task);

        return task;
    }
}