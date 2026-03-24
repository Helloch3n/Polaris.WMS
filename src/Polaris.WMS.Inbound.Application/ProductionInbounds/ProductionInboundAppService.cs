using Microsoft.AspNetCore.Authorization;
using Polaris.WMS.Inbound.Domain.ProductionInbounds;
using Polaris.WMS.Inound.Application.Contracts.ProductionInbounds;
using Polaris.WMS.Inound.Application.Contracts.ProductionInbounds.Dtos;
using Polaris.WMS.Integration.Departments;
using Polaris.WMS.Inventorys.Events;
using Polaris.WMS.MasterData.Application.Contracts.Integration.Products;
using Polaris.WMS.MasterData.Application.Contracts.Integration.Warehouses;
using Polaris.WMS.ProductionInbounds;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Identity.Settings;

namespace Polaris.WMS.Inbound.Application.ProductionInbounds
{
    /// <summary>
    /// 生产入库单应用服务
    /// </summary>
    [Authorize] // 确保只有登录用户可访问
    public class ProductionInboundAppService(
        ProductionInboundManager inboundManager,
        IRepository<ProductionInbound, Guid> inboundRepository,
        IDepartmentIntegrationService departmentIntegrationService,
        IRepository<ProductionInbound, Guid> productionInboundRepository,
        IWarehouseIntegrationService warehouseIntegrationService,
        IProductIntegrationService productIntegrationService,
        LocalEventBus localEventBus)
        : ApplicationService, IProductionInboundAppService
    {
        /// <summary>
        /// 审核入库单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task ApproveAndExecuteAsync(Guid id)
        {
            // 1. 加载入库单及明细
            var query = await productionInboundRepository.WithDetailsAsync(x => x.Details);
            var order = await AsyncExecuter.FirstOrDefaultAsync(query.Where(x => x.Id == id));
            var groupedDetails = order.Details.GroupBy(x => new
            {
                x.ProductionInboundId,
                x.ReelId,
                x.ActualLocationId
            }).ToList();
            foreach (var group in groupedDetails)
            {
                var orderId = group.Key.ProductionInboundId;
                var reelId = group.Key.ReelId;
                var actualLocationId = group.Key.ActualLocationId;
                await inboundManager.ReceiveReelAsync(orderId, reelId, actualLocationId);

                //发布待检入库事件
                await localEventBus.PublishAsync(new HoldInventoryCreatedEvent
                {
                    ContainerId = reelId,
                    CurrentLocationId = actualLocationId // 比如：收线机台的库位ID
                });
            }
        }

        /// <summary>
        /// 创建生产入库单
        /// </summary>
        public async Task<ProductionInboundDto> CreateAsync(CreateProductionInboundDto input)
        {
            // 1. 调用领域服务创建聚合根实体 (包含单号生成、查重验证、状态初始化)
            var inboundOrder = await inboundManager.CreateAsync(
                GuidGenerator.Create(),
                input.SourceOrderNo,
                input.SourceDepartmentId,
                input.TargetWarehouseId,
                input.InboundType // 传入入库类型
            );
            if (input.Details != null && input.Details.Count > 0)
            {
                foreach (var detail in input.Details)
                {
                    inboundOrder.AddDetail(
                        GuidGenerator.Create(),
                        inboundOrder.Id,
                        detail.ProductId,
                        detail.BatchNo,
                        detail.ReelId,
                        detail.Qty,
                        detail.Unit,
                        detail.Weight,
                        detail.SN,
                        detail.LayerIndex,
                        detail.RelatedOrderNo,
                        detail.RelatedOrderNoLineNo,
                        detail.ActualLocationId,
                        detail.CraftVersion
                    );
                }
            }

            // 2. 持久化到数据库
            await inboundRepository.InsertAsync(inboundOrder);

            // 3. 返回 DTO 给前端
            return ObjectMapper.Map<ProductionInbound, ProductionInboundDto>(inboundOrder);
        }

        /// <summary>
        /// 删除入库单及明细
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task DeleteAsync(Guid orderId)
        {
            await inboundManager.DeleteAsync(orderId);
        }

        /// <summary>
        /// 获取入库单及明细信息
        /// </summary>
        /// <returns></returns>
        public async Task<ProductionInboundDto> GetAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new UserFriendlyException("入库单号不能为空");
            }

            // 1. 查询入库单及明细
            var query = await productionInboundRepository.WithDetailsAsync(x => x.Details);
            var order = await AsyncExecuter.FirstOrDefaultAsync(query.Where(x => x.Id == id));
            if (order == null)
            {
                throw new UserFriendlyException("未找到对应的生产入库单");
            }

            // 2. 映射基础信息
            var dto = ObjectMapper.Map<ProductionInbound, ProductionInboundDto>(order);

            // 3. 补充来源部门与目标仓库展示字段
            var departmentDto = await departmentIntegrationService.GetAsync(order.SourceDepartmentId);
            var warehouseDto = await warehouseIntegrationService.GetAsync(order.TargetWarehouseId);
            // 4. 补充明细行的物料展示字段
            if (dto.Details is { Count: > 0 } details)
            {
                var productIds = details
                    .Select(x => x.ProductId)
                    .Where(x => x != Guid.Empty)
                    .Distinct()
                    .ToList();

                if (productIds.Count > 0)
                {
                    var productList = await productIntegrationService.GetListByIdsAsync(productIds);
                    var productMap = productList.ToDictionary(x => x.Id, x => x);

                    foreach (var detail in details)
                    {
                        if (productMap.TryGetValue(detail.ProductId, out var product))
                        {
                            detail.ProductName = product.Name;
                            detail.ProductCode = product.Code;
                        }
                    }
                }
            }

            dto.SourceDepartmentName = departmentDto.DisplayName;
            dto.SourceDepartmentCode = departmentDto.Code;
            dto.TargetWarehouseName = warehouseDto.Name;
            dto.TargetWarehouseCode = warehouseDto.Code;

            return dto;
        }

        /// <summary>
        /// 分页查询生产入库单列表
        /// </summary>
        public async Task<PagedResultDto<ProductionInboundDto>> GetListAsync(GetProductionInboundListDto input)
        {
            if (input == null) throw new UserFriendlyException("查询参数不能为空");

            // 1. 仅仅获取本模块的 IQueryable (干掉跨模块仓储)
            var inboundQuery = await inboundRepository.GetQueryableAsync();

            // 2. 过滤条件
            var filter = input.Filter?.Trim();
            inboundQuery = inboundQuery
                .WhereIf(!string.IsNullOrWhiteSpace(filter), x =>
                    (x.OrderNo != null && x.OrderNo.Contains(filter!)) ||
                    (x.SourceOrderNo != null && x.SourceOrderNo.Contains(filter!)))
                .WhereIf(input.InboundType.HasValue, x => x.InboundType == input.InboundType)
                .WhereIf(input.Status.HasValue, x => x.Status == input.Status)
                .WhereIf(input.SourceDepartmentId.HasValue, x => x.SourceDepartmentId == input.SourceDepartmentId)
                .WhereIf(input.TargetWarehouseId.HasValue, x => x.TargetWarehouseId == input.TargetWarehouseId);

            // 3. 总数统计
            var totalCount = await AsyncExecuter.CountAsync(inboundQuery);

            // 4. 安全排序与分页
            var sortingField = input.Sorting?.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            var isDesc = input.Sorting?.Contains("DESC", StringComparison.OrdinalIgnoreCase) == true;

            inboundQuery = sortingField switch
            {
                nameof(ProductionInbound.CreationTime) => isDesc
                    ? inboundQuery.OrderByDescending(x => x.CreationTime)
                    : inboundQuery.OrderBy(x => x.CreationTime),
                nameof(ProductionInbound.SourceOrderNo) => isDesc
                    ? inboundQuery.OrderByDescending(x => x.SourceOrderNo)
                    : inboundQuery.OrderBy(x => x.SourceOrderNo),
                nameof(ProductionInbound.Status) => isDesc
                    ? inboundQuery.OrderByDescending(x => x.Status)
                    : inboundQuery.OrderBy(x => x.Status),
                _ => inboundQuery.OrderByDescending(x => x.OrderNo)
            };

            var skipCount = input.SkipCount < 0 ? 0 : input.SkipCount;
            var maxResultCount = input.MaxResultCount <= 0 ? 10 : input.MaxResultCount;
            inboundQuery = inboundQuery.PageBy(skipCount, maxResultCount);

            //  5. 核心优化 1：只执行本模块的纯净查询
            var inbounds = await AsyncExecuter.ToListAsync(inboundQuery);
            if (!inbounds.Any())
                return new PagedResultDto<ProductionInboundDto>(totalCount, new List<ProductionInboundDto>());

            //  6. 核心优化 2：收集这一页数据中用到的依赖 ID（去重，极大地减少跨模块传输量）
            var deptIds = inbounds.Where(x => x.SourceDepartmentId != Guid.Empty).Select(x => x.SourceDepartmentId)
                .Distinct().ToList();
            var whIds = inbounds.Where(x => x.TargetWarehouseId != Guid.Empty).Select(x => x.TargetWarehouseId)
                .Distinct().ToList();

            //  7. 核心优化 3：并行发起跨模块批量请求！
            var deptTask = departmentIntegrationService.GetListByIdsAsync(deptIds);
            var whTask = warehouseIntegrationService.GetListByIdsAsync(whIds);
            await Task.WhenAll(deptTask, whTask);

            //  8. 核心优化 4：将结果转为内存字典，后续组装 O(1) 复杂度瞬间完成
            var deptMap = deptTask.Result.ToDictionary(x => x.Id, x => x);
            var whMap = whTask.Result.ToDictionary(x => x.Id, x => x);

            // 9. 内存极速映射组装
            var dtos = inbounds.Select(inbound =>
            {
                var dto = ObjectMapper.Map<ProductionInbound, ProductionInboundDto>(inbound);

                if (deptMap.TryGetValue(inbound.SourceDepartmentId, out var dept))
                {
                    dto.SourceDepartmentName = dept.DisplayName;
                    dto.SourceDepartmentCode = dept.Code;
                }

                if (whMap.TryGetValue(inbound.TargetWarehouseId, out var wh))
                {
                    dto.TargetWarehouseName = wh.Name;
                    dto.TargetWarehouseCode = wh.Code;
                }

                return dto;
            }).ToList();

            return new PagedResultDto<ProductionInboundDto>(totalCount, dtos);
        }

        public async Task<ProductionInboundDto> UpdateAsync(ProductionInboundDto input)
        {
            if (input.Id == Guid.Empty)
            {
                throw new BusinessException("调拨单Id不能为空");
            }

            // 第一步：加载当前实体（含明细）
            var query = await productionInboundRepository.WithDetailsAsync(x => x.Details);
            var order = await AsyncExecuter.FirstOrDefaultAsync(query.Where(x => x.Id == input.Id));
            if (order == null)
            {
                throw new BusinessException("调拨单不存在")
                    .WithData("调拨单Id", input.Id);
            }

            if (order.Status != ProductionInboundStatus.Draft)
            {
                throw new BusinessException("只有草稿状态的调拨单允许修改");
            }

            //var targetWarehouseId = input.TargetWarehouseId ?? Guid.Empty;
            //order.UpdateWarehouses(sourceWarehouseId, targetWarehouseId);

            // 第二步：记录更新前 Reel 集合，用于锁定差异分析
            var oldReelIds = order.Details.Select(x => x.ReelId).Distinct().ToHashSet();

            var incomingDetails = input.Details ?? new List<ProductionInboundDetailDto>();
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
                        detail.ProductionInboundId,
                        detail.ProductId,
                        detail.BatchNo,
                        detail.ReelId,
                        detail.Qty,
                        detail.Unit,
                        detail.Weight,
                        detail.SN,
                        detail.LayerIndex,
                        detail.RelatedOrderNo,
                        detail.RelatedOrderNoLineNo,
                        detail.ActualLocationId,
                        detail.CraftVersion);
                }
                else
                {
                    order.UpdateDetail(
                        detail.Id,
                        detail.ReelId,
                        detail.ProductId,
                        detail.Qty,
                        detail.ActualLocationId,
                        detail.CraftVersion);
                }
            }

            // 第六步：持久化并返回最新 DTO
            await productionInboundRepository.UpdateAsync(order, autoSave: true);
            return await GetAsync(order.Id);
        }
    }
}