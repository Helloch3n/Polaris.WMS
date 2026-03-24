namespace Polaris.WMS.DataSync
{
    /// <summary>
    /// 鏁版嵁鍚屾浠诲姟鎵ц鐘舵€?
    /// </summary>
    public enum SyncTaskStatus
    {
        /// <summary>
        /// 绌洪棽/鏈墽琛?
        /// </summary>
        Idle = 0,

        /// <summary>
        /// 鎵ц涓?
        /// </summary>
        Running = 1,

        /// <summary>
        /// 鎴愬姛
        /// </summary>
        Success = 2,

        /// <summary>
        /// 澶辫触
        /// </summary>
        Failed = 3
    }
}

