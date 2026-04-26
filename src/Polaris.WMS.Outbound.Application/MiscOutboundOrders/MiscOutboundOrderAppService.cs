using Polaris.WMS.Outbound.Application.Contracts.MiscOutboundOrders;
using Polaris.WMS.Outbound.Application.Contracts.MiscOutboundOrders.Dtos;
using Polaris.WMS.Outbound.Domain.MiscOrders;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Polaris.WMS.Outbound.Application.MiscOutboundOrders;

public class MiscOutboundOrderAppService(
    IRepository<MiscOutboundOrder, Guid> repository,
    MiscOutboundOrderManager miscOutboundOrderManager)
    : ApplicationService, IMiscOutboundOrderAppService
{
    /// <inheritdoc />
    public async Task ApproveAndExecuteAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new BusinessException("其他出库单Id不能为空。");
        }

        await miscOutboundOrderManager.ApproveAndExecuteAsync(id);
    }

    /// <inheritdoc />
    public async Task<MiscOutboundOrderDto> CreateAsync(CreateMiscOutboundOrderDto input)
    {
        var entity = MiscOutboundOrder.Create(
            GuidGenerator.Create(),
            input.OrderNo,
            input.AccountAliasId,
            input.AccountAliasDescription,
            input.CostCenterId,
            input.CostCenterCode,
            input.CostCenterName,
            input.Remark);

        foreach (var detail in input.Details)
        {
            entity.AddDetail(
                GuidGenerator.Create(),
                detail.WarehouseId,
                detail.WarehouseCode,
                detail.WarehouseName,
                detail.LocationId,
                detail.LocationCode,
                detail.ContainerId,
                detail.ContainerCode,
                detail.ProductId,
                detail.ProductCode,
                detail.ProductName,
                detail.SN,
                detail.BatchNo,
                detail.CraftVersion,
                detail.Unit,
                detail.Qty,
                detail.Remark);
        }

        await repository.InsertAsync(entity);
        return ObjectMapper.Map<MiscOutboundOrder, MiscOutboundOrderDto>(entity);
    }

    /// <inheritdoc />
    public async Task<MiscOutboundOrderDto> UpdateAsync(Guid id, UpdateMiscOutboundOrderDto input)
    {
        var query = await repository.WithDetailsAsync(x => x.Details);
        var entity = await AsyncExecuter.FirstOrDefaultAsync(query.Where(x => x.Id == id));
        if (entity == null)
        {
            throw new EntityNotFoundException(typeof(MiscOutboundOrder), id);
        }

        entity.UpdateHeader(
            input.AccountAliasId,
            input.AccountAliasDescription,
            input.CostCenterId,
            input.CostCenterCode,
            input.CostCenterName,
            input.Remark);

        foreach (var detail in entity.Details.ToList())
        {
            entity.RemoveDetail(detail.Id);
        }

        foreach (var detail in input.Details)
        {
            entity.AddDetail(
                GuidGenerator.Create(),
                detail.WarehouseId,
                detail.WarehouseCode,
                detail.WarehouseName,
                detail.LocationId,
                detail.LocationCode,
                detail.ContainerId,
                detail.ContainerCode,
                detail.ProductId,
                detail.ProductCode,
                detail.ProductName,
                detail.SN,
                detail.BatchNo,
                detail.CraftVersion,
                detail.Unit,
                detail.Qty,
                detail.Remark);
        }

        await repository.UpdateAsync(entity);
        return ObjectMapper.Map<MiscOutboundOrder, MiscOutboundOrderDto>(entity);
    }

    /// <inheritdoc />
    public async Task<MiscOutboundOrderDto> GetAsync(Guid id)
    {
        var query = await repository.WithDetailsAsync(x => x.Details);
        var entity = await AsyncExecuter.FirstOrDefaultAsync(query.Where(x => x.Id == id));
        if (entity == null)
        {
            throw new EntityNotFoundException(typeof(MiscOutboundOrder), id);
        }

        return ObjectMapper.Map<MiscOutboundOrder, MiscOutboundOrderDto>(entity);
    }

    /// <inheritdoc />
    public async Task<PagedResultDto<MiscOutboundOrderDto>> GetListAsync(MiscOutboundOrderSearchDto input)
    {
        var query = await repository.GetQueryableAsync();

        if (!string.IsNullOrWhiteSpace(input.OrderNo))
        {
            query = query.Where(x => x.OrderNo.Contains(input.OrderNo));
        }

        if (input.Status.HasValue)
        {
            query = query.Where(x => x.Status == input.Status.Value);
        }

        var totalCount = await AsyncExecuter.CountAsync(query);

        query = query
            .OrderByDescending(x => x.CreationTime)
            .PageBy(input.SkipCount, input.MaxResultCount);

        var entities = await AsyncExecuter.ToListAsync(query);
        var items = entities.Select(ObjectMapper.Map<MiscOutboundOrder, MiscOutboundOrderDto>).ToList();
        return new PagedResultDto<MiscOutboundOrderDto>(totalCount, items);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id)
    {
        await repository.DeleteAsync(id);
    }
}


