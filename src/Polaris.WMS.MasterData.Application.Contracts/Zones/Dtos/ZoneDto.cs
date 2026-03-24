using Polaris.WMS.MasterData.Warehouses;
using Volo.Abp.Application.Dtos;

namespace Polaris.WMS.MasterData.Application.Contracts.Zones.Dtos
{
    public class ZoneDto : AuditedEntityDto<Guid>
    {
        public Guid WarehouseId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public ZoneType ZoneType { get; set; }
    }
}

