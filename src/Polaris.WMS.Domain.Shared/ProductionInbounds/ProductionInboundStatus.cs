using System;
using System.Collections.Generic;
using System.Text;

namespace Polaris.WMS.ProductionInbounds
{
    /// <summary>
    /// 生产入库单状态枚举
    /// </summary>
    public enum ProductionInboundStatus
    {
        /// <summary>
        /// 草稿 (MES 推送过来，或手工刚建好，尚未开始入库作业)
        /// </summary>
        Draft = 0,

        /// <summary>
        /// 作业中 (部分盘具/明细已扫码入库到暂存区或正式区)
        /// </summary>
        InProgress = 1,

        /// <summary>
        /// 已完成 (所有明细均已完成入库)
        /// </summary>
        Completed = 2
    }
}
