using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Entities.Auditing;

namespace Polaris.WMS.Users
{
    /// <summary>
    /// 用户与管辖仓库的关联映射
    /// </summary>
    public class UserWarehouse : CreationAuditedEntity<Guid>
    {
        public Guid UserId { get; private set; }
        public Guid WarehouseId { get; private set; }

        protected UserWarehouse()
        {
        }

        public UserWarehouse(Guid id, Guid userId, Guid warehouseId) : base(id)
        {
            UserId = userId;
            WarehouseId = warehouseId;
        }
    }
}