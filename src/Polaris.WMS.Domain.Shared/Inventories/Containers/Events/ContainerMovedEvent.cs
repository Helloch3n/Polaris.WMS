using System;

namespace Polaris.WMS.MasterData.Containers.Events;

/// <summary>
/// 物理载具移库完成事件 (终点：叉车工搬运 Container 落地后触发)
/// </summary>
public class ReelMovedEvent
{
    // 1. 物理层面的位移信息 (属于 Container 的属性)
    public Guid ContainerId { get; set; }
    public Guid ActualNewLocationId { get; set; }
}