namespace Polaris.WMS.InventoryManage.Application.Contracts.Integration.Reels;

public class ReelIntegrationDto
{
    public Guid Id { get; set; }
    public string ReelNo { get; set; }

    public string Code { get; set; }

    public string Name { get; set; }

    public Guid? LocationId { get; set; }
}