using System;
using System.Collections.Generic;
using System.Text;

namespace Polaris.WMS.Isolation
{
    /// <summary>
    /// 部门/车间 隔离契约 (逻辑隔离/货主隔离)
    /// </summary>
    public interface IMultiDepartment
    {
        Guid DepartmentId { get; }
    }
}
