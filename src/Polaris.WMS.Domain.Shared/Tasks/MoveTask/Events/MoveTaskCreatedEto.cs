using System;
using Volo.Abp.EventBus;

namespace Polaris.WMS.Tasks.MoveTask.Events;

[EventName("Polaris.WMS.Task.MoveTaskCreated")]
public class MoveTaskCreatedEto
{
    public Guid TaskId { get; set; }
    public Guid? ContainerId { get; set; } // 如果任务关联了盘具容器
    public MoveTaskType TaskType { get; set; }
    public Guid FromLocationId { get; set; }
    public Guid TargetLocationId { get; set; }
}