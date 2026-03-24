namespace Polaris.WMS.MasterData.Domain.Integration.Inventory;

public class ReelInfo
{
    public Guid Id { get; set; }

    public string Code { get; set; }

    public string Name { get; set; }

    public Guid? LocationId { get; set; }
}