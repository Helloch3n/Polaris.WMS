using Polaris.WMS.MasterData.Warehouses;

namespace Polaris.WMS.MasterData.Application.Contracts.Zones.Dtos
{
    public class CreateUpdateZoneDto
    {
        public Guid WarehouseId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public ZoneType ZoneType { get; set; }
    }
}

