using Polaris.WMS.MasterData.Warehouses;

namespace Polaris.WMS.MasterData.Application.Contracts.Integration.Locations;

public class LocationIntegrationDto
{
    public Guid Id { get; set; }
    public Guid WarehouseId { get; set; }
    public string Code { get; set; }
    public LocationStatus Status { get; set; }
}