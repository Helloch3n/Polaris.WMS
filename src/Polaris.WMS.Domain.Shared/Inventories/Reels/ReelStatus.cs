namespace Polaris.WMS.Inventories.Reels
{
    /// <summary>
    /// 盘具状态。
    /// </summary>
    public enum ReelStatus
    {
        /// <summary>
        /// 空盘。
        /// </summary>
        Empty = 0,

        /// <summary>
        /// 占用中。
        /// </summary>
        Occupied = 1,

        /// <summary>
        /// 损坏。
        /// </summary>
        Damaged = 2,

        /// <summary>
        /// 维护中。
        /// </summary>
        Maintenance = 3
    }
}

