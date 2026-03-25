using Microsoft.AspNetCore.Authorization;
using Polaris.WMS.BillNumbers;
using Polaris.WMS.MasterData.Reels.Events;
using Polaris.WMS.TaskRouting.Application.Contracts.MoveTasks;
using Polaris.WMS.TaskRouting.Application.Contracts.MoveTasks.Dtos;
using Polaris.WMS.TaskRouting.Domain.Integration.Inventory;
using Polaris.WMS.TaskRouting.Domain.MoveTasks;
using Polaris.WMS.Tasks;
using Polaris.WMS.Tasks.MoveTask.Events;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.EventBus.Local;

namespace Polaris.WMS.TaskRouting.Application.MoveTasks
{
    [Authorize]
    public class MoveTaskAppService(
        IRepository<MoveTask, Guid> taskRepository,
        ILocalEventBus localEventBus,
        IDistributedEventBus distributedEventBus,
        IBillNumberGenerator billNumberGenerator,
        MoveTaskManager moveTaskManager,
        IExternalReelProvider externalReelProvider)
        : ApplicationService, IMoveTaskAppService
    {
        /// <summary>
        /// 生成搬运任务 (系统内部调用，外部接口禁止访问！)
        /// </summary>
        public async Task CreateAsync(CreateMoveTaskDto input)
        {
            // 防呆拦截：同一个托盘，不能有两个同时活着的搬运任务！
            var hasActiveTask = await taskRepository.AnyAsync(x =>
                x.ContainerId == input.ContainerId &&
                x.Status != MoveTaskStatus.Completed &&
                x.Status != MoveTaskStatus.Cancelled);

            if (hasActiveTask)
                throw new UserFriendlyException($"载具 {input.ContainerId} 已有正在执行的搬运任务，切勿重复派发！");

            var moveTask = await moveTaskManager.CreateMoveTaskAsync(input.TaskType, input.ContainerId,
                input.ContainerCode,
                input.FromLocationId,
                input.TargetLocationId);

            await taskRepository.InsertAsync(moveTask);

            await distributedEventBus.PublishAsync(new MoveTaskCreatedEto
            {
                TaskId = moveTask.Id,
                ContainerId = moveTask.ContainerId,
                TaskType = moveTask.TaskType,
                FromLocationId = moveTask.SourceLocationId,
                TargetLocationId = moveTask.TargetLocationId
            });
        }

        /// <summary>
        /// PDA 扫码确认搬运完成
        /// </summary>
        public async Task CompleteAsync(CompleteMoveTaskDto input)
        {
            // 1. 获取任务并锁状态
            var task = await taskRepository.GetAsync(input.TaskId);
            if (task.Status == MoveTaskStatus.Completed)
                throw new UserFriendlyException("任务已被其他人完成！");

            // 2. 执行物理移库 
            // 调用领域服务：校验目标库位容量 -> 更新 Inventory 表的 LocationId
            await externalReelProvider.MoveReelAsync(task.ContainerId, input.ScannedLocationId);

            // 3. 更新任务状态为“已完成”，并记录实际扫码的库位
            task.Complete(input.ScannedLocationId);
            await taskRepository.UpdateAsync(task);

            // 5. 抛出接力事件：货真正落地了！(用于触发 LIMS 接口)
            await distributedEventBus.PublishAsync(new ReelMovedEvent()
            {
                ReelId = task.ContainerId,
                ActualNewLocationId = input.ScannedLocationId, // 真实的落地位置 (QC大区)
            });
        }
    }
}