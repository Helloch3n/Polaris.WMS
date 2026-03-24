namespace Polaris.WMS.Inbound.Domain.Integration.Reels;

public class ExternalReelInfo
{
    public Guid Id { get; set; }

    public string Code { get; set; }

    public string Name { get; set; }

    public Guid? CurrentLocationId { get; set; }

    public string ReelNo { get; set; }
}