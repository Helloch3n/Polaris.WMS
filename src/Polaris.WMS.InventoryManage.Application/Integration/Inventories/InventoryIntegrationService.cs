using Polaris.WMS.InventoryManage.Application.Contracts.Integration.inventories;
using Polaris.WMS.InventoryManage.Domain.Containers;
using Polaris.WMS.InventoryManage.Domain.inventories;
using Polaris.WMS.InventoryManage.Domain.inventories.Args;
using Polaris.WMS.Inventories.Transaction;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Polaris.WMS.InventoryManage.Application.Integration.inventories;

[RemoteService(IsEnabled = false)]
public class InventoryIntegrationService(
    IRepository<Inventory, Guid> inventoryRepository,
    IRepository<Container, Guid> containerRepository,
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

    public async Task ShipAsync(ShipByContainerInput input)
    {
        if (input.ContainerId == Guid.Empty)
        {
            throw new BusinessException("容器Id不能为空。");
        }

        if (input.ProductId == Guid.Empty)
        {
            throw new BusinessException("物料Id不能为空。");
        }

        if (input.LocationId == Guid.Empty)
        {
            throw new BusinessException("库位Id不能为空。");
        }

        if (input.Qty <= 0)
        {
            throw new BusinessException("出库数量必须大于0。")
                .WithData("Qty", input.Qty);
        }

        var container = await containerRepository.GetAsync(input.ContainerId);
        if (container.CurrentLocationId != input.LocationId)
        {
            throw new BusinessException("容器当前库位与单据明细库位不一致，无法执行出库。")
                .WithData("ContainerId", input.ContainerId)
                .WithData("CurrentLocationId", container.CurrentLocationId)
                .WithData("ExpectedLocationId", input.LocationId);
        }

        var inventories = await inventoryRepository.GetListAsync(x =>
            x.ContainerId == input.ContainerId &&
            x.ProductId == input.ProductId &&
            x.SN == input.SN &&
            x.BatchNo == input.BatchNo);

        var targetInventory = inventories.OrderBy(x => x.CreationTime).FirstOrDefault();
        if (targetInventory == null)
        {
            throw new BusinessException("未找到匹配的库存记录，无法执行其他出库。")
                .WithData("ContainerId", input.ContainerId)
                .WithData("ProductId", input.ProductId)
                .WithData("SN", input.SN)
                .WithData("BatchNo", input.BatchNo)
                .WithData("DetailId", input.DetailId);
        }

        if (targetInventory.AvailableQuantity < input.Qty)
        {
            throw new BusinessException("库存可用数量不足，无法执行其他出库。")
                .WithData("InventoryId", targetInventory.Id)
                .WithData("AvailableQuantity", targetInventory.AvailableQuantity)
                .WithData("RequiredQuantity", input.Qty)
                .WithData("DetailId", input.DetailId);
        }

        targetInventory.Reserve(input.Qty);
        await inventoryRepository.UpdateAsync(targetInventory, autoSave: true);
        await inventoryManager.DeductQuantityAsync(targetInventory.Id, input.Qty, TransactionType.Issue, input.OrderNo);
    }
}