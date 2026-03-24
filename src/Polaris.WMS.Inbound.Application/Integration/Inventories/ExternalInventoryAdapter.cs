using Polaris.WMS.Inbound.Domain.Integration.Inventories;
using Polaris.WMS.InventoryManage.Application.Contracts.Integration.inventories;
using Volo.Abp.DependencyInjection;

namespace Polaris.WMS.Inbound.Application.Integration.Inventories;

public class ExternalInventoryAdapter(
    IInventoryIntegrationService inventoryIntegrationService
) : IExternalInventoryAdapter, ITransientDependency
{
    public Task<List<ExternalInventoryInfo>> GetInventoryByReels(List<Guid> reelIds)
    {
        throw new NotImplementedException();
    }

    public async Task ReceiveProductionAsync(ExternalProductionReceiveInfo info)
    {
        var dto = new ProductionReceiveIntegrationDto
        {
            OrderNo = info.OrderNo,
            ReelId = info.ReelId,
            ProductId = info.ProductId,
            Qty = info.Qty,
            Weight = info.Weight,
            BatchNo = info.BatchNo,
            RelatedOrderNo = info.RelatedOrderNo,
            RelatedOrderNoLineNo = info.RelatedOrderNoLineNo,
            ActualLocationId = info.ActualLocationId,
            SN = info.SN,
            Unit = info.Unit,
            CraftVersion = info.CraftVersion,
            LayerIndex = info.LayerIndex,
            Status = info.Status
        };

        await inventoryIntegrationService.ReceiveProductionAsync(dto);
    }
}