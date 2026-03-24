using System;
using System.Collections.Generic;
using System.Text;

namespace Polaris.WMS.ProductionInbounds
{
    /// <summary>
    /// 生产入库类型
    /// </summary>
    public enum ProductionInboundType
    {
        /// <summary>
        /// 成品入库 (最终完工，通常入成品仓，准备发货)
        /// </summary>
        FinishedProduct = 10,

        /// <summary>
        /// 半成品入库 (可作为独立产品流转或外卖，通常入半成品仓)
        /// </summary>
        SemiFinishedProduct = 20,

        /// <summary>
        /// 工序品/在制品入库 (仅在车间工序间流转暂存，如：拉丝后的裸铜线待绞线)
        /// </summary>
        WorkInProgress = 30
    }
}
