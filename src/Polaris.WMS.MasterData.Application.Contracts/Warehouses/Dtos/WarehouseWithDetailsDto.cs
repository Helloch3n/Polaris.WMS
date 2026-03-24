using Polaris.WMS.MasterData.Application.Contracts.Locations.Dtos;

namespace Polaris.WMS.MasterData.Application.Contracts.Warehouses.Dtos
{
    public class WarehouseWithDetailsDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public List<ZoneWithLocationsDto> Zones { get; set; } = [];
    }

    public class ZoneWithLocationsDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public List<LocationDto> Locations { get; set; } = [];
    }
}
