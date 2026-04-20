namespace Polaris.WMS.Inbound.Domain.Integration.Containers;

public class ExternalContainerInfo
{
    public Guid Id { get; set; }

    public string Code { get; set; }

    public string Name { get; set; }

    public Guid? CurrentLocationId { get; set; }

    public string ContainerCode { get; set; }
}