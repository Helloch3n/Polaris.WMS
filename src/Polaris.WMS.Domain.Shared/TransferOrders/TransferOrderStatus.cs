using System;
using System.Collections.Generic;
using System.Text;

namespace Polaris.WMS.TransferOrders
{
    public enum TransferOrderStatus
    {
        Draft = 0,          // 草稿
        Approved = 1,       // 已审核（等待作业）
        InProgress = 2,     // 作业中（部分明细已完成）
        Completed = 3,      // 已完成
        Cancelled = 4       // 已取消
    }
}
