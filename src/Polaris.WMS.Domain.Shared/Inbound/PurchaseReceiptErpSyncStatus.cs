namespace Polaris.WMS.Inbound;
/// <summary>
/// 表示暂收（Purchase Receipt）与外部 ERP 系统同步的状态。
/// 设计说明：
/// - 正数表示已成功或已完成的阶段；负数表示该阶段同步失败并可能需要人工或重试处理。
/// - 该枚举用于在入库流程中精确记录“暂收”与 ERP 同步的两个关键阶段（Receive / Deliver）。
/// </summary>
public enum PurchaseReceiptErpSyncStatus
{
    /// <summary>
    /// 未同步（初始状态）。
    /// </summary>
    NotSynced = 0,

    /// <summary>
    /// 暂收同步到 ERP 失败（卡在第一步：Receive）。
    /// 处理建议：检查网络、请求参数和 ERP 返回信息，必要时重试或人工干预。
    /// </summary>
    ReceiveFailed = -1,

    /// <summary>
    /// 暂收已同步到 ERP（Receive 成功）。
    /// </summary>
    Received = 1,

    /// <summary>
    /// 入库（Deliver）同步到 ERP 失败（卡在第二步）。
    /// 隐含含义：暂收的 Receive 阶段已成功，但 Deliver 阶段失败。
    /// 处理建议：执行补偿/回滚或人工复核后重试 Deliver。
    /// </summary>
    DeliverFailed = -2,

    /// <summary>
    /// 入库已同步到 ERP（Deliver 成功）。
    /// </summary>
    Delivered = 2
}