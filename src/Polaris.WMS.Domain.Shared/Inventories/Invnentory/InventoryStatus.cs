namespace Polaris.WMS.Inventories.Invnentory
{
    /// <summary>
    /// 库存状态。
    /// </summary>
    public enum InventoryStatus
    {
        /// <summary>
        /// 良品。
        /// </summary>
        Good = 0,

        /// <summary>
        /// 冻结。
        /// </summary>
        Frozen = 1,

        /// <summary>
        /// 待检
        /// </summary>
        Hold = 2,

        /// <summary>
        /// 待检。
        /// </summary>
        Quarantine = 3,

        /// <summary>
        /// 报废。
        /// </summary>
        Scrap = 4
    }
}