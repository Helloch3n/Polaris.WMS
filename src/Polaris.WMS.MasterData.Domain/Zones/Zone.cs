using Polaris.WMS.MasterData.Warehouses;
using Volo.Abp.Domain.Entities.Auditing;

namespace Polaris.WMS.MasterData.Domain.Zones
{
    public class Zone : FullAuditedAggregateRoot<Guid>
    {
        public string Code { get; private set; }
        public string Name { get; private set; }
        public ZoneType ZoneType { get; private set; }

        public Guid WarehouseId { get; private set; }

        protected Zone()
        {
        }

        internal Zone(
            Guid id,
            Guid warehouseId,
            string code,
            string name,
            ZoneType zoneType) : base(id)
        {
            WarehouseId = warehouseId;
            Code = code;
            Name = name;
            ZoneType = zoneType;
        }

        public void Update(string code, string name, ZoneType zoneType)
        {
            Code = code;
            Name = name;
            ZoneType = zoneType;
        }
    }
}

