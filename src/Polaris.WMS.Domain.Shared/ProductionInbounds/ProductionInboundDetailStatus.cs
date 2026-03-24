using System;
using System.Collections.Generic;
using System.Text;

namespace Polaris.WMS.ProductionInbounds
{
    /// 生产入库明细状态枚举
    /// </summary>
    public enum ProductionInboundDetailStatus
    {
        /// <summary>
        /// 待入库 (等待扫码确权)
        /// </summary>
        Pending = 0,

        /// <summary>
        /// 已入库 (已经落入暂存库位或正式库位，库存已增加)
        /// </summary>
        Completed = 1
    }
}
