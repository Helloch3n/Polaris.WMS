using System;
using System.Collections.Generic;
using System.Text;

namespace Polaris.WMS.Isolation
{
    public interface IWMSContextProvider
    {
        Guid? CurrentDepartmentId { get; }
        Guid? CurrentWarehouseId { get; }
    }
}
