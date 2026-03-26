using Polaris.WMS.Tasks;
using Polaris.WMS.Tasks.MoveTask.Events;
using Volo.Abp.Domain.Entities.Auditing;

namespace Polaris.WMS.TaskRouting.Domain.MoveTasks;

/// <summary>
/// 搬运任务聚合根：指挥现场物理载具移动的唯一凭证
/// </summary>
public class MoveTask : FullAuditedAggregateRoot<Guid>
{
    public string TaskNo { get; private set; }
    public MoveTaskType TaskType { get; private set; }
    public MoveTaskStatus Status { get; private set; }

    /// <summary> 绑定的物理载体</summary>
    public Guid ContainerId { get; private set; }

    public string ContainerCode { get; private set; }

    public Guid SourceLocationId { get; private set; }

    /// <summary> 计划目标库位 (系统推荐去的地方，比如 QC大区) </summary>
    public Guid TargetLocationId { get; private set; }

    /// <summary> 实际落位库位 (完成时由工人拿 PDA 真实扫出来的条码) </summary>
    public Guid ActualLocationId { get; private set; }

    protected MoveTask()
    {
    } // 供 EF Core 序列化使用

    internal MoveTask(Guid id, string taskNo, MoveTaskType taskType, Guid containerId, string containerCode,
        Guid sourceLocationId,
        Guid targetLocationId)
        : base(id)
    {
        TaskNo = taskNo;
        TaskType = taskType;
        ContainerId = containerId;
        ContainerCode = containerCode;
        SourceLocationId = sourceLocationId;
        TargetLocationId = targetLocationId;
        Status = MoveTaskStatus.Pending; // 初始状态必为待执行
    }

    public static MoveTask Create(Guid id, string taskNo, MoveTaskType taskType, Guid containerId, string containerCode,
        Guid sourceLocationId,
        Guid targetLocationId)
    {
        return new MoveTask(id, taskNo, taskType, containerId, containerCode, sourceLocationId, targetLocationId);
    }

    // 认领任务 (可选步骤，防止两个叉车工抢同一个盘子)
    public void Claim()
    {
        if (Status != MoveTaskStatus.Pending)
            throw new InvalidOperationException($"任务 {TaskNo} 当前状态无法认领！");
        Status = MoveTaskStatus.InProgress;
    }

    // 完成任务
    public void Complete(Guid actualLocationId)
    {
        if (Status == MoveTaskStatus.Completed || Status == MoveTaskStatus.Cancelled)
            throw new InvalidOperationException($"任务 {TaskNo} 已结束，无法再次完成！");

        ActualLocationId = actualLocationId;
        Status = MoveTaskStatus.Completed;
    }

    public void Cancel()
    {
        Status = MoveTaskStatus.Cancelled;
    }

    /// <summary>
    /// 触发“搬运任务已创建”的分布式事件
    /// </summary>
    public void GenerateCreatedEvent()
    {
        // 因为是在实体内部，这里可以合法访问 protected 的 AddDistributedEvent 方法
        AddLocalEvent(new MoveTaskCreatedEto
        {
            TaskId = this.Id,
            ContainerId = this.ContainerId,
            TaskType = this.TaskType,
            FromLocationId = this.SourceLocationId, // 注意匹配你实体的属性名
            TargetLocationId = this.TargetLocationId
        });
    }
}