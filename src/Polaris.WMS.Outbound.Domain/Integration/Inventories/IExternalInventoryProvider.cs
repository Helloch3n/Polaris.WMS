using Volo.Abp.DependencyInjection;

namespace Polaris.WMS.Outbound.Domain.Integration.Inventories;

public interface IExternalInventoryProvider : ITransientDependency
{
    Task ShipAsync(ExternalMiscOutboundShipInfo info);
}

