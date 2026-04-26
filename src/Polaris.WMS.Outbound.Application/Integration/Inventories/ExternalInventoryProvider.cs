using Polaris.WMS.InventoryManage.Application.Contracts.Integration.inventories;
using Polaris.WMS.Outbound.Domain.Integration.Inventories;
using Volo.Abp.DependencyInjection;

namespace Polaris.WMS.Outbound.Application.Integration.Inventories;

public class ExternalInventoryProvider(
    IInventoryIntegrationService inventoryIntegrationService)
    : IExternalInventoryProvider, ITransientDependency
{
    public async Task ShipAsync(ExternalMiscOutboundShipInfo info)
    {
        var input = new ShipByContainerInput
        {
            OrderNo = info.OrderNo,
            DetailId = info.DetailId,
            ContainerId = info.ContainerId,
            LocationId = info.LocationId,
            ProductId = info.ProductId,
            Qty = info.Qty,
            BatchNo = info.BatchNo,
            SN = info.SN
        };

        await inventoryIntegrationService.ShipAsync(input);
    }
}

