using Polaris.WMS.InventoryManage.Application.Contracts.Integration.inventories;
using Polaris.WMS.InventoryManage.Domain.inventories;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Polaris.WMS.InventoryManage.Application.Integration.inventories;

[RemoteService(IsEnabled = false)]
public class InventoryIntegrationService(
    IRepository<Inventory, Guid> inventoryRepository,
    InventoryManager inventoryManager)
    : ApplicationService, IInventoryIntegrationService
{
    public async Task<List<InventoryIntegrationDto>> GetInventoryByReels(List<Guid> reelIds)
    {
        // 1. 边界防护：防止传入空集合导致 EF Core 生成无效的 WHERE IN () SQL
        if (reelIds == null || !reelIds.Any())
        {
            return new List<InventoryIntegrationDto>();
        }

        // 2. 核心查询：使用 Queryable 组合 IN 查询
        // 假设你在构造函数中注入了 IRepository<Inventory, Guid> inventoryRepository
        var query = await inventoryRepository.GetQueryableAsync();

        // 注意：这里的 x.ReelId 请替换为你库存实体中实际关联盘具的字段名（比如 ContainerId）
        var inventories = await AsyncExecuter.ToListAsync(query.Where(x => reelIds.Contains(x.ReelId)));

        // 3. 结果组装：映射为外部模块需要的 DTO
        return inventories.Select(x => new InventoryIntegrationDto
        {
            Id = x.Id,
        }).ToList();
    }

    public async Task ReceiveProductionAsync(ProductionReceiveIntegrationDto input)
    {
        await inventoryManager.ProductionReceiveAsync(
            input.OrderNo,
            input.ReelId,
            input.ProductId,
            input.Qty,
            input.Weight,
            input.BatchNo,
            input.RelatedOrderNo,
            input.RelatedOrderNoLineNo,
            input.ActualLocationId,
            input.SN,
            input.Unit,
            input.CraftVersion,
            input.LayerIndex,
            input.Status
        );
    }
}