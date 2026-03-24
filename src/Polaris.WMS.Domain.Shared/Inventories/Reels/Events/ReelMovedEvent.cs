using System;
using Polaris.WMS.Inventorys;

namespace Polaris.WMS.MasterData.Reels.Events;

/// <summary>
/// 物理载具移库完成事件 (终点：叉车工搬运 Reel 落地后触发)
/// </summary>
public class ReelMovedEvent
{
    // 1. 物理层面的位移信息 (属于 Reel 的属性)
    public Guid ReelId { get; set; }
    public Guid ActualNewLocationId { get; set; }
}