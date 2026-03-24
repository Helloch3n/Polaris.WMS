using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Polaris.WMS.InventoryManage.Application.Contracts.InventoryTransactions.Dtos;
using Polaris.WMS.InventoryManage.Domain.inventories;
using Polaris.WMS.InventoryManage.Domain.Reels;
using Polaris.WMS.MasterData.Application.Contracts.Integration.Locations;
using Polaris.WMS.MasterData.Application.Contracts.Integration.Products;
using Polaris.WMS.MasterData.Application.Contracts.Integration.Warehouses;
using Polaris.WMS.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Polaris.WMS.InventoryManage.Application.InventoryTransactions
{
    [Authorize(WMSPermissions.InventoryOps.Default)]
    public class InventoryTransactionAppService(
        IRepository<InventoryTransaction, System.Guid> transactionRepository,
        IRepository<Reel, Guid> reelRepository,
        InventoryMapper mapper,
        IProductIntegrationService productIntegrationService,
        ILocationIntegrationService locationIntegrationService,
        IWarehouseIntegrationService warehouseIntegrationService)
        : ApplicationService, IApplicationService
    {
        [Authorize(WMSPermissions.InventoryOps.InventoryTransactions.View)]
        public async Task<PagedResultDto<InventoryTransactionDto>> GetListAsync(InventoryTransactionSearchDto input)
        {
            // 🌟 1. 斩断 WithDetailsAsync，只获取最纯净的本表 Queryable
            var query = await transactionRepository.GetQueryableAsync();

            // 🌟 2. 处理跨实体的条件过滤 (ReelNo)
            if (!string.IsNullOrWhiteSpace(input.ReelNo))
            {
                var reelQuery = await reelRepository.GetQueryableAsync();
                var reelIdsQuery = reelQuery
                    .Where(x => x.ReelNo.Contains(input.ReelNo!))
                    .Select(x => x.Id);

                // 将原本的 JOIN 过滤替换为 IN 过滤
                query = query.Where(x => reelIdsQuery.Contains(x.ReelId));
            }

            // 3. 处理本表的常规过滤
            query = query
                .WhereIf(!string.IsNullOrWhiteSpace(input.BillNo), x => x.BillNo.Contains(input.BillNo!))
                .WhereIf(input.ProductId.HasValue, x => x.ProductId == input.ProductId!.Value)
                .WhereIf(input.Type.HasValue, x => x.Type == input.Type!.Value)
                .WhereIf(input.StartTime.HasValue, x => x.CreationTime >= input.StartTime!.Value)
                .WhereIf(input.EndTime.HasValue, x => x.CreationTime <= input.EndTime!.Value);

            // 4. 统计总数并分页排序
            var totalCount = await AsyncExecuter.CountAsync(query);

            if (!string.IsNullOrWhiteSpace(input.Sorting))
            {
                query = query.OrderBy(input.Sorting);
            }
            else
            {
                query = query.OrderByDescending(x => x.CreationTime);
            }

            query = query.PageBy(input.SkipCount, input.MaxResultCount);
            var items = await AsyncExecuter.ToListAsync(query);

            if (!items.Any())
            {
                return new PagedResultDto<InventoryTransactionDto>(totalCount, new List<InventoryTransactionDto>());
            }

            // 5. 基础实体字段映射（依赖刚刚改好的 Mapper，此时展示字段为空）
            var dtos = items.Select(mapper.Map).ToList();

            // 🌟 6. 收集所需的跨模块/跨聚合 ID 集合 (使用 HashSet 去重，极致榨干性能)
            var reelIds = items.Select(x => x.ReelId).Distinct().ToList();
            var productIds = items.Select(x => x.ProductId).Distinct().ToList();
            
            // 合并 FromLocationId 和 ToLocationId
            var locationIds = items.Where(x => x.FromLocationId.HasValue).Select(x => x.FromLocationId!.Value)
                .Concat(items.Where(x => x.ToLocationId.HasValue).Select(x => x.ToLocationId!.Value))
                .Distinct().ToList();

            // 合并 FromWarehouseId 和 ToWarehouseId
            var warehouseIds = items.Where(x => x.FromWarehouseId.HasValue).Select(x => x.FromWarehouseId!.Value)
                .Concat(items.Where(x => x.ToWarehouseId.HasValue).Select(x => x.ToWarehouseId!.Value))
                .Distinct().ToList();

            // 🌟 7. 并行发起外部查询 (Task.WhenAll 是微服务组装的神器)
            var reelQueryForMap = await reelRepository.GetQueryableAsync();
            var reelsTask = AsyncExecuter.ToListAsync(reelQueryForMap.Where(x => reelIds.Contains(x.Id)));
            var productsTask = productIntegrationService.GetListByIdsAsync(productIds);
            var locationsTask = locationIntegrationService.GetListByIdsAsync(locationIds);
            var warehousesTask = warehouseIntegrationService.GetListByIdsAsync(warehouseIds);

            await Task.WhenAll(reelsTask, productsTask, locationsTask, warehousesTask);

            // 🌟 8. 转换为 O(1) 复杂度的哈希字典
            var reelMap = reelsTask.Result.ToDictionary(x => x.Id, x => x);
            var productMap = productsTask.Result.ToDictionary(x => x.Id, x => x);
            var locationMap = locationsTask.Result.ToDictionary(x => x.Id, x => x);
            var warehouseMap = warehousesTask.Result.ToDictionary(x => x.Id, x => x);

            // 🌟 9. 内存极速拼装
            foreach (var dto in dtos)
            {
                // 拼装 ReelNo
                if (reelMap.TryGetValue(dto.ReelId, out var reel))
                {
                    dto.ReelNo = reel.ReelNo;
                }

                // 拼装 ProductName
                if (productMap.TryGetValue(dto.ProductId, out var product))
                {
                    dto.ProductName = product.Name;
                }

                // 拼装 FromLocationCode 和 ToLocationCode
                if (dto.FromLocationId.HasValue && locationMap.TryGetValue(dto.FromLocationId.Value, out var fromLoc))
                {
                    dto.FromLocationCode = fromLoc.Code;
                }
                if (dto.ToLocationId.HasValue && locationMap.TryGetValue(dto.ToLocationId.Value, out var toLoc))
                {
                    dto.ToLocationCode = toLoc.Code;
                }

                // 拼装 FromWarehouseCode 和 ToWarehouseCode
                if (dto.FromWarehouseId.HasValue && warehouseMap.TryGetValue(dto.FromWarehouseId.Value, out var fromWh))
                {
                    dto.FromWarehouseCode = fromWh.Code;
                }
                if (dto.ToWarehouseId.HasValue && warehouseMap.TryGetValue(dto.ToWarehouseId.Value, out var toWh))
                {
                    dto.ToWarehouseCode = toWh.Code;
                }
            }

            return new PagedResultDto<InventoryTransactionDto>(totalCount, dtos);
        }
    }
}
