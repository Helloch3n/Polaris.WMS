using Polaris.WMS.MasterData.Warehouses;

namespace Polaris.WMS.InventoryManage.Domain.Integration.Locations;

public class ExternalLocationInfo
{
    public Guid Id { get; set; }
    public Guid WarehouseId { get; set; }
    public string Code { get; set; }
    public LocationStatus Status { get; set; }
}