using System;
using System.Collections.Generic;
using System.Text;

namespace Polaris.WMS.MasterData.Containers
{
    /// <summary>
    /// 盘具业务类型 (决定了盘具的生命周期和流转规则)
    /// </summary>
    public enum ContainerType
    {
        /// <summary>
        /// 周转盘 (厂内固定资产，循环使用，库存清空后变为空盘)
        /// </summary>
        Turnover = 0,

        /// <summary>
        /// 成品盘 (用于发货，通常由消耗完的原料盘转化而来，随成品一起出库)
        /// </summary>
        FinishedGoods = 1,

        /// <summary>
        /// 虚拟盘 (用于无盘散件的系统占位，库存清空后自动销毁)
        /// </summary>
        Virtual = 2
    }
}