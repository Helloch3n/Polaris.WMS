namespace Polaris.WMS.Inventories.Transaction
{
    /// <summary>
    /// 事务类型枚举（用于标识库存流水的业务类型）。
    /// 说明：
    /// - Receipt: 入库/收货
    /// - Issue: 出库/发货
    /// - Transfer: 库内调拨/移位
    /// - Adjust: 库存调整（盘点或人工调整）
    /// </summary>
    public enum TransactionType
    {
        /// <summary>
        /// 收货/入库事务（增加库存）
        /// </summary>
        Receipt = 0,

        /// <summary>
        /// 发货/出库事务（减少库存）
        /// </summary>
        Issue = 1,

        /// <summary>
        /// 库内调拨或移位（在不同库位之间移动库存，不改变总量）
        /// </summary>
        Transfer = 2,

        /// <summary>
        /// 库存调整（例如盘点差异或手工修正）
        /// </summary>
        Adjust = 3,

        /// <summary>
        /// 材料投入
        /// </summary>
        Feed = 4,

        /// <summary>
        /// 材料退回
        /// </summary>
        Return = 5
    }
}