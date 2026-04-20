using Microsoft.AspNetCore.Authorization;
using Polaris.WMS.Integration.Departments;
using Polaris.WMS.InventoryManage.Application.Contracts.TransferOrders;
using Polaris.WMS.InventoryManage.Application.Contracts.TransferOrders.Dtos;
using Polaris.WMS.InventoryManage.Domain.Containers;
using Polaris.WMS.InventoryManage.Domain.TransferOrders;
using Polaris.WMS.Isolation;
using Polaris.WMS.MasterData.Application.Contracts.Integration.Locations;
using Polaris.WMS.MasterData.Application.Contracts.Integration.Products;
using Polaris.WMS.MasterData.Application.Contracts.Integration.Warehouses;
using Polaris.WMS.Permissions;
using Polaris.WMS.TransferOrders;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity.Settings;

namespace Polaris.WMS.InventoryManage.Application.TransferOrders
{
    /// <summary>
    /// 调拨单应用服务。
    /// </summary>
    public class TransferAppService(
        IRepository<TransferOrder, Guid> transferOrderRepository,
        IRepository<Container, Guid> containerRepository,
        TransferOrderManager transferOrderManager,
        TransferMappers transferMapper,
        IWMSContextProvider wmsContextProvider,
        IProductIntegrationService productIntegrationService,
        ILocationIntegrationService locationIntegrationService,
        IWarehouseIntegrationService warehouseIntegrationService,
        IDepartmentIntegrationService departmentIntegrationService)
        : ApplicationService, ITransferAppService
    {
        /// <summary>
        /// 获取调拨单信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<TransferDto> GetAsync(Guid id)
        {
            var query = await transferOrderRepository.WithDetailsAsync(x => x.Details);
            var entity = await AsyncExecuter.FirstOrDefaultAsync(query.Where(x => x.Id == id));
            if (entity == null)
            {
                throw new BusinessException("调拨单不存在").WithData("调拨单Id", id);
            }

            var dto = transferMapper.Map(entity);

            // 1. 收集所有需要的 ID
            var containerIds = dto.Details.Select(x => x.ContainerId).Distinct().ToList();
            var productIds = dto.Details.Select(x => x.ProductId).Distinct().ToList();
            var locationIds = dto.Details.SelectMany(x => new[] { x.SourceLocationId, x.TargetLocationId }).Distinct()
                .ToList();

            // 2. 本地查询 Container，跨模块并行查询 Product 和 Location
            var reelQuery = await containerRepository.GetQueryableAsync();
            var reels = await AsyncExecuter.ToListAsync(reelQuery.Where(x => containerIds.Contains(x.Id)));
            var products = await productIntegrationService.GetListByIdsAsync(productIds);
            var locations = await locationIntegrationService.GetListByIdsAsync(locationIds);

            var reelMap = reels.ToDictionary(x => x.Id, x => x.ContainerCode);
            var productMap = products.ToDictionary(x => x.Id, x => x);
            var locationMap = locations.ToDictionary(x => x.Id, x => x);

            // 3. 根据库位结果，收集并跨模块查询 Warehouse
            var warehouseIds = locations.Select(x => x.WarehouseId).Distinct().ToList();
            var warehouses = await warehouseIntegrationService.GetListByIdsAsync(warehouseIds);
            var warehouseMap = warehouses.ToDictionary(x => x.Id, x => x.Code);

            // 4. 内存极速组装
            foreach (var detail in dto.Details)
            {
                if (reelMap.TryGetValue(detail.ContainerId, out var containerCode)) detail.ReelCode = containerCode;

                if (productMap.TryGetValue(detail.ProductId, out var product))
                {
                    detail.ProductCode = product.Code;
                    detail.ProductName = product.Name;
                }

                if (locationMap.TryGetValue(detail.SourceLocationId, out var sourceLocation))
                {
                    detail.SourceLocationCode = sourceLocation.Code;
                    detail.SourceWarehouseId = sourceLocation.WarehouseId;
                    if (warehouseMap.TryGetValue(sourceLocation.WarehouseId, out var sourceWarehouseCode))
                    {
                        detail.SourceWarehouseCode = sourceWarehouseCode;
                    }
                }

                if (locationMap.TryGetValue(detail.TargetLocationId, out var targetLocation))
                {
                    detail.TargetLocationCode = targetLocation.Code;
                    detail.TargetWarehouseId = targetLocation.WarehouseId;
                    if (warehouseMap.TryGetValue(targetLocation.WarehouseId, out var targetWarehouseCode))
                    {
                        detail.TargetWarehouseCode = targetWarehouseCode;
                    }
                }
            }

            dto.SourceWarehouseId = entity.SourceWarehouseId;
            dto.TargetWarehouseId = entity.TargetWarehouseId;

            if (!dto.SourceWarehouseId.HasValue)
                dto.SourceWarehouseId = dto.Details.Select(x => x.SourceWarehouseId).FirstOrDefault(x => x.HasValue);
            if (!dto.TargetWarehouseId.HasValue)
                dto.TargetWarehouseId = dto.Details.Select(x => x.TargetWarehouseId).FirstOrDefault(x => x.HasValue);

            if (dto.SourceWarehouseId.HasValue &&
                warehouseMap.TryGetValue(dto.SourceWarehouseId.Value, out var sourceCode))
                dto.SourceWarehouseCode = sourceCode;
            if (dto.TargetWarehouseId.HasValue &&
                warehouseMap.TryGetValue(dto.TargetWarehouseId.Value, out var targetCode))
                dto.TargetWarehouseCode = targetCode;

            return dto;
        }


        [Authorize(WMSPermissions.InternalOps.TransferOrders.Default)]
        public async Task<PagedResultDto<TransferListDto>> GetListAsync(TransferSearchDto input)
        {
            var query = await transferOrderRepository.GetQueryableAsync();

            query = query
                .WhereIf(!string.IsNullOrWhiteSpace(input.OrderNo), x => x.OrderNo.Contains(input.OrderNo!))
                .WhereIf(input.Status.HasValue, x => x.Status == input.Status!.Value)
                .WhereIf(input.StartTime.HasValue, x => x.CreationTime >= input.StartTime!.Value)
                .WhereIf(input.EndTime.HasValue, x => x.CreationTime <= input.EndTime!.Value);

            var totalCount = await AsyncExecuter.CountAsync(query);
            var items = await AsyncExecuter.ToListAsync(query.OrderByDescending(x => x.CreationTime)
                .PageBy(input.SkipCount, input.MaxResultCount));

            if (!items.Any()) return new PagedResultDto<TransferListDto>(totalCount, new List<TransferListDto>());

            var warehouseIds = items.Select(x => x.WarehouseId).Distinct().ToList();
            var departmentIds = items.Select(x => x.DepartmentId).Distinct().ToList();

            // 🌟 并行跨模块查询仓库和部门
            var warehousesTask = warehouseIntegrationService.GetListByIdsAsync(warehouseIds);
            var departmentsTask = departmentIntegrationService.GetListByIdsAsync(departmentIds);

            await Task.WhenAll(warehousesTask, departmentsTask);

            var warehouseMap = warehousesTask.Result.ToDictionary(x => x.Id, x => x);
            var departmentMap = departmentsTask.Result.ToDictionary(x => x.Id, x => x);

            var dtos = items.Select(x =>
            {
                var dto = new TransferListDto
                {
                    Id = x.Id,
                    OrderNo = x.OrderNo,
                    Status = x.Status,
                    CreationTime = x.CreationTime,
                    WarehouseId = x.WarehouseId,
                    DepartmentId = x.DepartmentId
                };

                if (warehouseMap.TryGetValue(x.WarehouseId, out var wh))
                {
                    dto.WarehouseCode = wh.Code;
                    dto.WarehouseName = wh.Name;
                }

                if (departmentMap.TryGetValue(x.DepartmentId, out var dept))
                {
                    dto.DepartmentCode = dept.Code;
                    dto.DepartmentName = dept.DisplayName;
                }

                return dto;
            }).ToList();

            return new PagedResultDto<TransferListDto>(totalCount, dtos);
        }

        /// <summary>
        /// 调拨单创建
        /// </summary>
        /// <param name="input"></param>    
        /// <returns></returns>
        [Authorize(WMSPermissions.InternalOps.TransferOrders.Create)]
        public async Task<TransferDto> CreateAsync(CreateTransferDto input)
        {
            // 从上下文中获取当前操作者所在的部门和仓库
            var currentDeptId = wmsContextProvider.CurrentDepartmentId;
            var currentWarehouseId = wmsContextProvider.CurrentWarehouseId;

            // 校验上下文中获取当前操作者所在的部门和仓库是否等于单据的来源仓库和所属部门
            if (currentWarehouseId.HasValue && currentWarehouseId.Value != input.SourceWarehouseId)
            {
                throw new UserFriendlyException("非法操作：您当前处于特定仓库视图，不允许替其他仓库创建调拨单！");
            }

            if (currentDeptId.HasValue && currentDeptId.Value != input.SourceDepartmentId)
            {
                throw new UserFriendlyException("非法操作：您当前处于特定部门视图，不允许替其他部门创建调拨单！");
            }

            // 第一步：创建调拨单表头（含源/目标仓库）
            var order = await transferOrderManager.CreateAsync(
                GuidGenerator.Create(),
                input.SourceWarehouseId,
                input.TargetWarehouseId,
                input.SourceDepartmentId);

            // 第二步：如果入参带明细，则同步创建明细
            foreach (var detail in input.Details)
            {
                order.AddDetail(
                    GuidGenerator.Create(),
                    detail.ContainerId,
                    detail.InventoryId,
                    detail.ProductId,
                    detail.Qty,
                    detail.SourceLocationId,
                    detail.TargetLocationId);
            }

            // 第三步：锁定本次调拨单涉及的盘具
            var containerIds = input.Details
                .Select(x => x.ContainerId)
                .Distinct()
                .ToList();

            if (containerIds.Count > 0)
            {
                var reelQuery = await containerRepository.GetQueryableAsync();
                var reels = await AsyncExecuter.ToListAsync(reelQuery.Where(x => containerIds.Contains(x.Id)));

                foreach (var container in reels)
                {
                    if (container.IsLocked)
                    {
                        throw new BusinessException("存在已锁定盘具，无法创建调拨单")
                            .WithData("ContainerId", container.Id)
                            .WithData("ContainerCode", container.ContainerCode);
                    }

                    container.Lock($"调拨单锁定：{order.OrderNo}");
                    await containerRepository.UpdateAsync(container);
                }
            }

            // 第四步：持久化单据
            await transferOrderRepository.InsertAsync(order, autoSave: true);

            // 第五步：查询并返回最新单据详情
            var query = await transferOrderRepository.WithDetailsAsync(x => x.Details);
            var created = await AsyncExecuter.FirstAsync(query.Where(x => x.Id == order.Id));

            return transferMapper.Map(created);
        }

        /// <summary>
        /// 调拨单更新（前端全量回传，后端执行差异分析）。
        /// </summary>
        //[Authorize(WMSPermissions.TransferOrder.Update)]
        public async Task<TransferDto> UpdateAsync(TransferDto input)
        {
            if (input.Id == Guid.Empty)
            {
                throw new BusinessException("调拨单Id不能为空");
            }

            // 第一步：加载当前实体（含明细）
            var query = await transferOrderRepository.WithDetailsAsync(x => x.Details);
            var order = await AsyncExecuter.FirstOrDefaultAsync(query.Where(x => x.Id == input.Id));
            if (order == null)
            {
                throw new BusinessException("调拨单不存在")
                    .WithData("调拨单Id", input.Id);
            }

            if (order.Status != TransferOrderStatus.Draft)
            {
                throw new BusinessException("只有草稿状态的调拨单允许修改");
            }

            var sourceWarehouseId = input.SourceWarehouseId ?? Guid.Empty;
            var targetWarehouseId = input.TargetWarehouseId ?? Guid.Empty;
            order.UpdateWarehouses(sourceWarehouseId, targetWarehouseId);

            // 第二步：记录更新前 Container 集合，用于锁定差异分析
            var oldContainerIds = order.Details.Select(x => x.ContainerId).Distinct().ToHashSet();

            var incomingDetails = input.Details ?? new List<TransferDetailDto>();
            var incomingDetailIds = incomingDetails
                .Where(x => x.Id != Guid.Empty)
                .Select(x => x.Id)
                .ToHashSet();

            // 第三步：删除前端移除的明细
            var removedDetailIds = order.Details
                .Where(x => !incomingDetailIds.Contains(x.Id))
                .Select(x => x.Id)
                .ToList();

            foreach (var detailId in removedDetailIds)
            {
                order.RemoveDetail(detailId);
            }

            // 第四步：新增或更新前端回传的明细
            var existingDetailIds = order.Details.Select(x => x.Id).ToHashSet();
            foreach (var detail in incomingDetails)
            {
                if (detail.Id == Guid.Empty || !existingDetailIds.Contains(detail.Id))
                {
                    order.AddDetail(
                        GuidGenerator.Create(),
                        detail.ContainerId,
                        detail.InventoryId,
                        detail.ProductId,
                        detail.Qty,
                        detail.SourceLocationId,
                        detail.TargetLocationId);
                }
                else
                {
                    order.UpdateDetail(
                        detail.Id,
                        detail.ContainerId,
                        detail.InventoryId,
                        detail.ProductId,
                        detail.Qty,
                        detail.SourceLocationId,
                        detail.TargetLocationId);
                }
            }

            // 第五步：对 Container 锁定做差异分析（新增锁定、移除解锁）
            var newContainerIds = order.Details.Select(x => x.ContainerId).Distinct().ToHashSet();
            var toLockContainerIds = newContainerIds.Except(oldContainerIds).ToList();
            var toUnlockContainerIds = oldContainerIds.Except(newContainerIds).ToList();

            var changedContainerIds = toLockContainerIds.Union(toUnlockContainerIds).Distinct().ToList();
            if (changedContainerIds.Count > 0)
            {
                var reelQuery = await containerRepository.GetQueryableAsync();
                var reels = await AsyncExecuter.ToListAsync(reelQuery.Where(x => changedContainerIds.Contains(x.Id)));

                foreach (var container in reels)
                {
                    if (toLockContainerIds.Contains(container.Id))
                    {
                        if (container.IsLocked)
                        {
                            throw new BusinessException("存在已锁定盘具，无法更新调拨单")
                                .WithData("ContainerId", container.Id)
                                .WithData("ContainerCode", container.ContainerCode);
                        }

                        container.Lock($"调拨单锁定：{order.OrderNo}");
                        await containerRepository.UpdateAsync(container);
                    }

                    if (toUnlockContainerIds.Contains(container.Id) && container.IsLocked)
                    {
                        container.UnLock();
                        await containerRepository.UpdateAsync(container);
                    }
                }
            }

            // 第六步：持久化并返回最新 DTO
            await transferOrderRepository.UpdateAsync(order, autoSave: true);
            return await GetAsync(order.Id);
        }

        /// <summary>
        /// 调拨单审核
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(WMSPermissions.InternalOps.TransferOrders.Approve)]
        public async Task ApproveAndExecuteAsync(Guid id)
        {
            // 第一步：调用领域服务执行审核与调拨
            await transferOrderManager.ApproveAndExecuteAsync(id);
        }

        /// <summary>
        /// 调拨单删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(Guid id)
        {
            // 第一步：调用领域服务执行删除规则（状态校验、解锁、级联删除）
            await transferOrderManager.DeleteAsync(id);
        }
    }
}