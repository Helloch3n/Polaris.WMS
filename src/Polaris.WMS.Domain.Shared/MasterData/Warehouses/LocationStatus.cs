namespace Polaris.WMS.MasterData.Warehouses
{
    public enum LocationStatus
    {
        /// <summary>
        /// 空闲
        /// </summary>
        Idle = 0,

        /// <summary>
        /// 部分占用 (有货，但还没到 MaxReelCount)
        /// </summary>
        Partial = 10,

        /// <summary>
        /// 已满
        /// </summary>
        Full = 20,

        /// <summary>
        /// 锁定 (盘点中、货架维修中，禁止一切出入库)
        /// </summary>
        Locked = 30
    }
}

