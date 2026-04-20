namespace Polaris.WMS.InventoryManage.Application.Contracts.Integration.Containers;

public class ContainerIntegrationDto
{
    public Guid Id { get; set; }
    public string ContainerCode { get; set; }

    public string Code { get; set; }

    public string Name { get; set; }

    public Guid? LocationId { get; set; }
}