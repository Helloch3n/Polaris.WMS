using System;
using System.Collections.Generic;
using System.Text;

namespace Polaris.WMS.Isolation
{
    /// <summary>
    /// 仓库 隔离契约 (物理隔离/作业隔离)
    /// </summary>
    public interface IMultiWarehouse
    {
        Guid WarehouseId { get; }
    }
}
