using Polaris.WMS.MasterData.Warehouses;

namespace Polaris.WMS.TaskRouting.Domain.Integration.MasterData;

public class ExternalLocationInfo
{
    public Guid Id { get; set; }
    public Guid WarehouseId { get; set; }
    public string Code { get; set; }
    public LocationStatus Status { get; set; }
}