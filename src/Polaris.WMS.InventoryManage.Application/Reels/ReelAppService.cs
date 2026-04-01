using Polaris.WMS.InventoryManage.Application.Contracts.Reels;
using Polaris.WMS.InventoryManage.Application.Contracts.Reels.Dtos;
using Polaris.WMS.InventoryManage.Domain.inventories;
using Polaris.WMS.InventoryManage.Domain.Reels;
using Polaris.WMS.MasterData.Application.Contracts.Integration.Locations;
using Polaris.WMS.MasterData.Application.Contracts.Integration.Products;
using Polaris.WMS.MasterData.Application.Contracts.Integration.Warehouses;
using Polaris.WMS.MasterData.Application.Contracts.Integration.Zones;
using Polaris.WMS.MasterData.Application.Contracts.Locations;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Polaris.WMS.InventoryManage.Application.Reels
{
    public class ReelAppService(
        IRepository<Reel, Guid> repository,
        IRepository<Inventory, Guid> inventoryRepository,
        ReelManager reelManager,
        ILocationIntegrationService locationIntegrationService,
        IProductIntegrationService productIntegrationService,
        IZoneIntegrationService zoneIntegrationService,
        IWarehouseIntegrationService warehouseIntegrationService)
        :
            CrudAppService<Reel, ReelDto, Guid, ReelSearchDto, CreateUpdateReelDto>(repository),
            IReelAppService
    {
        /// <summary>
        /// 获取可搬运盘具列表（按来源仓库过滤未锁定盘具）。
        /// </summary>
        public async Task<List<MovableReelDto>> GetMovableReelsAsync(Guid sourceWarehouseId)
        {
            if (sourceWarehouseId == Guid.Empty)
            {
                throw new UserFriendlyException("来源仓库不能为空。");
            }

            // 第一步：跨模块呼叫，获取来源仓库下的所有库位 ID
            var sourceLocationIds =
                await locationIntegrationService.GetLocationIdsByWarehouseIdAsync(sourceWarehouseId);

            if (sourceLocationIds == null || !sourceLocationIds.Any())
            {
                return new List<MovableReelDto>(); // 如果仓库下没库位，直接返回
            }

            // 第二步：查询可搬运盘具（未锁定且在来源仓库库位下）
            var reelQuery = await Repository.GetQueryableAsync();
            var reels = await AsyncExecuter.ToListAsync(
                reelQuery.Where(x =>
                    !x.IsLocked &&
                    x.CurrentLocationId.HasValue &&
                    sourceLocationIds.Contains(x.CurrentLocationId.Value)));

            if (reels.Count == 0)
            {
                return new List<MovableReelDto>();
            }

            // 第三步：批量查询盘具下库存 (本地模块，合法)
            var reelIds = reels.Select(x => x.Id).ToList();
            var inventoryQuery = await inventoryRepository.GetQueryableAsync();
            var inventories = await AsyncExecuter.ToListAsync(inventoryQuery.Where(x => reelIds.Contains(x.ReelId)));

            // 🌟 第四步：并行加载跨模块展示数据 (极大压榨性能)
            var productIds = inventories.Select(x => x.ProductId).Distinct().ToList();
            var locationIds = reels.Where(x => x.CurrentLocationId.HasValue).Select(x => x.CurrentLocationId!.Value)
                .Distinct().ToList();

            var productsTask = productIntegrationService.GetListByIdsAsync(productIds);
            var locationsTask = locationIntegrationService.GetListByIdsAsync(locationIds);

            await Task.WhenAll(productsTask, locationsTask);

            var productMap = productsTask.Result.ToDictionary(x => x.Id, x => x);
            var locationMap = locationsTask.Result.ToDictionary(x => x.Id, x => x.Code);

            // 第五步：O(1) 内存组装返回 DTO
            var result = reels.Select(reel =>
            {
                var items = inventories
                    .Where(x => x.ReelId == reel.Id)
                    .Select(x =>
                    {
                        productMap.TryGetValue(x.ProductId, out var product);
                        return new InventoryBriefDto
                        {
                            InventoryId = x.Id,
                            ProductId = x.ProductId,
                            ProductCode = product?.Code,
                            ProductName = product?.Name,
                            BatchNo = x.BatchNo,
                            Quantity = x.Quantity,
                            Uom = x.Unit
                        };
                    })
                    .ToList();

                var currentLocationId = reel.CurrentLocationId ?? Guid.Empty;
                var currentLocationCode = reel.CurrentLocationId.HasValue &&
                                          locationMap.TryGetValue(reel.CurrentLocationId.Value, out var code)
                    ? code
                    : string.Empty;

                return new MovableReelDto
                {
                    ReelId = reel.Id,
                    ReelNo = reel.ReelNo,
                    ReelType = reel.ReelType.ToString(),
                    CurrentLocationId = currentLocationId,
                    CurrentLocationCode = currentLocationCode,
                    Inventories = items
                };
            }).ToList();

            return result;
        }

        /// <summary>
        /// 创建盘具并返回结果（不补充库位编码）。
        /// </summary>
        public override async Task<ReelDto> CreateAsync(CreateUpdateReelDto input)
        {
            await CheckCreatePolicyAsync();

            // 1. 跨聚合前置校验：通过防腐接口校验库位是否存在 (代替原先违规的 locationAppService)
            if (input.CurrentLocationId.HasValue)
            {
                var location = await locationIntegrationService.GetAsync(input.CurrentLocationId.Value);
                if (location == null)
                {
                    throw new UserFriendlyException("指定的当前库位不存在。");
                }
            }

            // 2. 业务规则下沉：调用领域服务创建盘具
            var id = GuidGenerator.Create();
            var entity = await reelManager.CreateAsync(
                id,
                input.ReelNo,
                input.Name,
                input.Size,
                input.SelfWeight,
                input.CurrentLocationId,
                input.ReelType);

            // 3. 持久化并返回 DTO
            await Repository.InsertAsync(entity);
            return ObjectMapper.Map<Reel, ReelDto>(entity);
        }

        /// <summary>
        /// 更新盘具当前库位。
        /// </summary>
        public async Task UpdateLocationAsync(Guid id, Guid newLocationId)
        {
            // 1. 校验目标库位 (使用防腐接口)
            var location = await locationIntegrationService.GetAsync(newLocationId);
            if (location == null)
            {
                throw new UserFriendlyException("目标库位不存在。");
            }

            // 2. 加载并更新聚合状态
            var entity = await Repository.GetAsync(id);

            entity.SetLocation(newLocationId);

            await Repository.UpdateAsync(entity);
        }

        /// <summary>
        /// 更新盘具基础信息，并按需更新库位。
        /// </summary>
        public override async Task<ReelDto> UpdateAsync(Guid id, CreateUpdateReelDto input)
        {
            await CheckUpdatePolicyAsync();

            var entity = await Repository.GetAsync(id);

            // 1. 跨聚合前置校验：通过防腐接口校验库位是否存在
            if (input.CurrentLocationId.HasValue)
            {
                var location = await locationIntegrationService.GetAsync(input.CurrentLocationId.Value);
                if (location == null)
                {
                    throw new UserFriendlyException("指定的当前库位不存在。");
                }
            }

            var reelNo = string.IsNullOrWhiteSpace(input.ReelNo)
                ? entity.ReelNo
                : input.ReelNo;

            // 2. 业务规则下沉：调用领域服务更新盘具基础信息
            await reelManager.UpdateAsync(
                entity,
                reelNo,
                input.Name,
                input.Size,
                input.SelfWeight);

            // 3. 处理本用例下的库位变更
            if (input.CurrentLocationId.HasValue)
            {
                entity.SetLocation(input.CurrentLocationId.Value);
            }

            await Repository.UpdateAsync(entity);
            return ObjectMapper.Map<Reel, ReelDto>(entity);
        }

        /// <summary>
        /// 分页查询盘具列表，并补充库位/区域/仓库信息。
        /// </summary>
        public override async Task<PagedResultDto<ReelDto>> GetListAsync(ReelSearchDto input)
        {
            var queryable = await Repository.GetQueryableAsync();

            if (!string.IsNullOrWhiteSpace(input.ContainerCode))
            {
                queryable = queryable.Where(x => x.ReelNo == input.ContainerCode);
            }

            var totalCount = await AsyncExecuter.CountAsync(queryable);

            var items = await AsyncExecuter.ToListAsync(
                queryable
                    .OrderBy(x => x.ReelNo)
                    .PageBy(input.SkipCount, input.MaxResultCount));

            // 1. 实体映射
            var dtos = items.Select(x => ObjectMapper.Map<Reel, ReelDto>(x)).ToList();
            // 2. 补充跨聚合展示字段（库位/区域/仓库编码）
            await PopulateLocationDetailsAsync(dtos);

            return new PagedResultDto<ReelDto>(totalCount, dtos);
        }

        /// <summary>
        /// 根据主键获取盘具实体。
        /// </summary>
        protected override async Task<Reel> GetEntityByIdAsync(Guid id)
        {
            var query = await Repository.GetQueryableAsync();
            var entity = query.FirstOrDefault(x => x.Id == id);
            return entity;
        }

        /// <summary>
        /// 获取单个盘具，并补充库位编码。
        /// </summary>
        public override async Task<ReelDto> GetAsync(Guid id)
        {
            await CheckGetPolicyAsync();

            // 1. 读取聚合
            var entity = await GetEntityByIdAsync(id);
            // 2. 实体映射
            var dto = ObjectMapper.Map<Reel, ReelDto>(entity);

            // 3. 通过防腐接口获取库位编码 (替换掉 _locationRepository)
            if (dto.CurrentLocationId.HasValue)
            {
                var location = await locationIntegrationService.GetAsync(dto.CurrentLocationId.Value);
                dto.CurrentLocationCode = location?.Code;
            }

            return dto;
        }

        /// <summary>
        /// 批量补充盘具 DTO 的库位、区域与仓库属性。
        /// </summary>
        private async Task PopulateLocationDetailsAsync(List<ReelDto> dtos)
        {
            var locationIds = dtos
                .Where(x => x.CurrentLocationId.HasValue)
                .Select(x => x.CurrentLocationId!.Value)
                .Distinct()
                .ToList();

            if (locationIds.Count == 0)
            {
                return;
            }

            // 🌟 1. 跨模块获取 Location 信息
            var locations = await locationIntegrationService.GetListByIdsAsync(locationIds);
            var locationMap = locations.ToDictionary(x => x.Id, x => x);

            var zoneIds = locations.Select(x => x.Id).Distinct().ToList();
            var warehouseIds = locations.Select(x => x.WarehouseId).Distinct().ToList();

            // 🌟 2. 并行跨模块获取 Zone 和 Warehouse 信息
            var zonesTask = zoneIntegrationService.GetListByIdsAsync(zoneIds);
            var warehousesTask = warehouseIntegrationService.GetListByIdsAsync(warehouseIds);

            await Task.WhenAll(zonesTask, warehousesTask);
            var zoneMap = zonesTask.Result.ToDictionary(x => x.Id, x => x.Code);
            var warehouseMap = warehousesTask.Result.ToDictionary(x => x.Id, x => x.Code);

            // 🌟 3. O(1) 内存极速组装
            foreach (var dto in dtos)
            {
                if (!dto.CurrentLocationId.HasValue)
                {
                    continue;
                }

                if (!locationMap.TryGetValue(dto.CurrentLocationId.Value, out var location))
                {
                    continue;
                }

                dto.CurrentLocationCode = location.Code;
                dto.CurrentZoneId = location.Id;
                dto.CurrentWarehouseId = location.WarehouseId;

                if (zoneMap.TryGetValue(location.Id, out var zoneCode))
                {
                    dto.CurrentZoneCode = zoneCode;
                }

                if (warehouseMap.TryGetValue(location.WarehouseId, out var warehouseCode))
                {
                    dto.CurrentWarehouseCode = warehouseCode;
                }
            }
        }
    }
}