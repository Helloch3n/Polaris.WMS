namespace Polaris.WMS.Tasks;

public enum MoveTaskStatus
{
    Pending = 10, // 待执行 (刚生成，还没人接单)
    InProgress = 20, // 执行中 (工人扫码锁定了这个任务)
    Completed = 30, // 已完成
    Cancelled = 40 // 已取消
}