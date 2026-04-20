using Polaris.WMS.InventoryManage.Application.Contracts.Integration.inventories;
using Polaris.WMS.InventoryManage.Domain.inventories;
using Polaris.WMS.InventoryManage.Domain.inventories.Args;
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
    public async Task<List<InventoryIntegrationDto>> GetInventoryByContainers(List<Guid> containerIds)
    {
        // 1. 边界防护：防止传入空集合导致 EF Core 生成无效的 WHERE IN () SQL
        if (containerIds == null || !containerIds.Any())
        {
            return new List<InventoryIntegrationDto>();
        }

        // 2. 核心查询：使用 Queryable 组合 IN 查询
        // 假设你在构造函数中注入了 IRepository<Inventory, Guid> inventoryRepository
        var query = await inventoryRepository.GetQueryableAsync();

        // 注意：这里的 x.ContainerId 请替换为你库存实体中实际关联盘具的字段名（比如 ContainerId）
        var inventories = await AsyncExecuter.ToListAsync(query.Where(x => containerIds.Contains(x.ContainerId)));

        // 3. 结果组装：映射为外部模块需要的 DTO
        return inventories.Select(x => new InventoryIntegrationDto
        {
            Id = x.Id,
        }).ToList();
    }

    public async Task ReceiveProductionAsync(ProductionReceiveByContainerInput input)
    {
        var domainArgs = new ReceiveContainerArgs
        {
            OrderNo = input.OrderNo,
            ContainerId = input.ContainerId,
            LocationId = input.LocationId,
            Items = input.Items.Select(x => new ReceiveContainerItemArgs
            {
                ProductId = x.ProductId,
                Qty = x.Qty,
                Weight = x.Weight,
                BatchNo = x.BatchNo,
                RelatedOrderNo = x.RelatedOrderNo,
                RelatedOrderLineNo = x.RelatedOrderLineNo,
                SN = x.SN,
                Unit = x.Unit,
                CraftVersion = x.CraftVersion,
                LayerIndex = x.LayerIndex,
                Status = x.Status
            }).ToList()
        };
        await inventoryManager.ReceiveByContainerAsync(domainArgs);
    }
}