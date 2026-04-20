using Polaris.WMS.Inbound.Domain.Integration.Inventories;
using Polaris.WMS.InventoryManage.Application.Contracts.Integration.inventories;
using Volo.Abp.DependencyInjection;

namespace Polaris.WMS.Inbound.Application.Integration.Inventories;

public class ExternalInventoryProvider(
    IInventoryIntegrationService inventoryIntegrationService
) : IExternalInventoryProvider, ITransientDependency
{
    public Task<List<ExternalInventoryInfo>> GetInventoryByReels(List<Guid> reelIds)
    {
        throw new NotImplementedException();
    }

    public async Task ReceiveProductionAsync(ExternalProductionReceiveInfo info)
    {
        var dto = new ProductionReceiveByContainerInput
        {
            OrderNo = info.OrderNo,
            ContainerId = info.ReelId,
            LocationId = info.ActualLocationId,
        };
        foreach (var item in info.Items)
        {
            dto.Items.Add(new ReceiveItemDto
            {
                ProductId = item.ProductId,
                Qty = item.Qty,
                Weight = item.Weight,
                BatchNo = item.BatchNo,
                RelatedOrderNo = item.RelatedOrderNo,
                RelatedOrderLineNo = item.RelatedOrderNoLineNo,
                SN = item.SN,
                Unit = item.Unit,
                CraftVersion = item.CraftVersion,
                LayerIndex = item.LayerIndex,
                Status = item.Status
            });
        }

        await inventoryIntegrationService.ReceiveProductionAsync(dto);
    }
}