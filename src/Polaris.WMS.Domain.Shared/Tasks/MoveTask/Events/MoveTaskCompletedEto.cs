using System;
using Volo.Abp.EventBus;

namespace Polaris.WMS.Tasks.MoveTask.Events;

[EventName("Polaris.WMS.TaskRouting.MoveTaskCompleted")]
public class MoveTaskCompletedEto
{
    public Guid TaskId { get; set; }
    public Guid ContainerId { get; set; } // 盘具ID
    public string ContainerCode { get; set; }
    public Guid FromLocationId { get; set; }
    public Guid ActualLocationId { get; set; } // 实际放下的库位
    public MoveTaskType TaskType { get; set; }
}