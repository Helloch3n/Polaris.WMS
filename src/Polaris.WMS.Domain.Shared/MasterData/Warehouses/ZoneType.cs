namespace Polaris.WMS.MasterData.Warehouses
{
    public enum ZoneType
    {
        /// <summary>
        /// 收货暂存区 (地面暂存)
        /// </summary>
        Dock = 10,

        /// <summary>
        /// 正式存储区 (钢平台/货场)
        /// </summary>
        Storage = 20,

        /// <summary>
        /// 生产线边库 (车间暂存)
        /// </summary>
        LineSide = 30,

        /// <summary>
        /// 质检/隔离区
        /// </summary>
        QC = 40,

        /// <summary>
        /// 发货暂存区
        /// </summary>
        Outbound = 50
    }
}