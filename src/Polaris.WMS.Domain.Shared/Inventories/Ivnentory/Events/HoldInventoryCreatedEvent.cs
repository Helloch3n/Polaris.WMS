using System;

namespace Polaris.WMS.Inventorys.Events;

/// <summary>
/// 待检库存生成事件 (当物料入库且状态为 Hold 时触发)
/// </summary>
public class HoldInventoryCreatedEvent
{
    /// <summary>
    /// 容器号/盘号 (LIMS 质检员扫码用的唯一标识)
    /// </summary>
    public Guid ContainerId { get; set; }

    /// <summary>
    /// 目前所在的物理位置 (机台收线位)
    /// </summary>
    public Guid CurrentLocationId { get; set; }
}