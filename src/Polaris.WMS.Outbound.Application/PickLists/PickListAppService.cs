using Polaris.WMS.MasterData.Application.Contracts.Integration.Locations;
using Polaris.WMS.Outbound.Application.Contracts.PickLists;
using Polaris.WMS.Outbound.Domain;
using Polaris.WMS.Outbound.Domain.PickLists;
using Polaris.WMS.Outbound.Domain.SalesShipments;
using Polaris.WMS.TaskRouting.Application.Contracts.Integration.PickTasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Polaris.WMS.Outbound.Application.PickLists;

public class PickListAppService(
    IRepository<PickList, Guid> pickListRepository,
    IRepository<SalesShipmentAllocation, Guid> salesShipmentAllocationRepository,
    OutboundFulfillmentManager outboundFulfillmentManager,
    ILocationIntegrationService locationIntegrationService,
    ITaskRoutingIntegrationService taskRoutingIntegrationService)
    : ApplicationService, IPickListAppService
{
    /// <inheritdoc />
    public async Task<PickListDto> CreateAsync(CreatePickListDto input)
    {
        var location = await locationIntegrationService.GetLocationInfoByCodeAsync(input.TargetLocationCode.Trim());
        var entity = await outboundFulfillmentManager.CreatePickListAsync(input.WaveOrderId, location.Id, location.Code, input.Remark);
        return MapToDto(entity);
    }

    /// <inheritdoc />
    public async Task ReleaseAsync(Guid id)
    {
        var query = await pickListRepository.WithDetailsAsync(x => x.Lines);
        var entity = await AsyncExecuter.FirstOrDefaultAsync(query.Where(x => x.Id == id));
        if (entity == null)
        {
            throw new EntityNotFoundException(typeof(PickList), id);
        }

        foreach (var line in entity.Lines.Where(x => !x.MoveTaskId.HasValue))
        {
            var task = await taskRoutingIntegrationService.CreatePickDownTaskAsync(new CreatePickDownTaskInput
            {
                ContainerId = line.ContainerId,
                ContainerCode = line.ContainerCode,
                SourceLocationId = line.SourceLocationId,
                SourceLocationCode = line.SourceLocationCode,
                TargetLocationId = line.TargetLocationId,
                TargetLocationCode = line.TargetLocationCode
            });

            entity.AttachTask(line.Id, task.TaskId, task.TaskNo);

            var allocation = await salesShipmentAllocationRepository.GetAsync(line.SalesShipmentAllocationId);
            if (allocation.Status == SalesShipmentAllocationStatus.Reserved)
            {
                allocation.MarkTaskCreated();
                await salesShipmentAllocationRepository.UpdateAsync(allocation, autoSave: true);
            }
        }

        await pickListRepository.UpdateAsync(entity, autoSave: true);
    }

    /// <inheritdoc />
    public async Task<PickListDto> GetAsync(Guid id)
    {
        var query = await pickListRepository.WithDetailsAsync(x => x.Lines);
        var entity = await AsyncExecuter.FirstOrDefaultAsync(query.Where(x => x.Id == id));
        if (entity == null)
        {
            throw new EntityNotFoundException(typeof(PickList), id);
        }

        return MapToDto(entity);
    }

    /// <inheritdoc />
    public async Task<PagedResultDto<PickListDto>> GetListAsync(PickListSearchDto input)
    {
        var query = await pickListRepository.GetQueryableAsync();

        if (!string.IsNullOrWhiteSpace(input.PickNo))
        {
            query = query.Where(x => x.PickNo.Contains(input.PickNo));
        }

        if (input.Status.HasValue)
        {
            query = query.Where(x => x.Status == input.Status.Value);
        }

        if (input.WaveOrderId.HasValue)
        {
            query = query.Where(x => x.WaveOrderId == input.WaveOrderId.Value);
        }

        var totalCount = await AsyncExecuter.CountAsync(query);
        var items = await AsyncExecuter.ToListAsync(query.OrderByDescending(x => x.CreationTime).PageBy(input.SkipCount, input.MaxResultCount));
        return new PagedResultDto<PickListDto>(totalCount, items.Select(MapToListDto).ToList());
    }

    private static PickListDto MapToDto(PickList source)
    {
        return new PickListDto
        {
            Id = source.Id,
            PickNo = source.PickNo,
            WaveOrderId = source.WaveOrderId,
            TargetLocationId = source.TargetLocationId,
            TargetLocationCode = source.TargetLocationCode,
            Status = source.Status,
            Remark = source.Remark,
            CreationTime = source.CreationTime,
            CreatorId = source.CreatorId,
            LastModificationTime = source.LastModificationTime,
            LastModifierId = source.LastModifierId,
            Lines = source.Lines.Select(x => new PickListLineDto
            {
                Id = x.Id,
                PickListId = x.PickListId,
                SalesShipmentId = x.SalesShipmentId,
                SalesShipmentNo = x.SalesShipmentNo,
                SalesShipmentDetailId = x.SalesShipmentDetailId,
                SalesShipmentAllocationId = x.SalesShipmentAllocationId,
                ProductId = x.ProductId,
                ProductCode = x.ProductCode,
                ProductName = x.ProductName,
                Qty = x.Qty,
                ContainerId = x.ContainerId,
                ContainerCode = x.ContainerCode,
                SourceLocationId = x.SourceLocationId,
                SourceLocationCode = x.SourceLocationCode,
                TargetLocationId = x.TargetLocationId,
                TargetLocationCode = x.TargetLocationCode,
                MoveTaskId = x.MoveTaskId,
                MoveTaskNo = x.MoveTaskNo,
                ActualLocationId = x.ActualLocationId,
                BatchNo = x.BatchNo,
                SN = x.SN,
                IsPicked = x.IsPicked
            }).ToList()
        };
    }

    private static PickListDto MapToListDto(PickList source)
    {
        return new PickListDto
        {
            Id = source.Id,
            PickNo = source.PickNo,
            WaveOrderId = source.WaveOrderId,
            TargetLocationId = source.TargetLocationId,
            TargetLocationCode = source.TargetLocationCode,
            Status = source.Status,
            Remark = source.Remark,
            CreationTime = source.CreationTime,
            CreatorId = source.CreatorId,
            LastModificationTime = source.LastModificationTime,
            LastModifierId = source.LastModifierId,
            Lines = new List<PickListLineDto>()
        };
    }
}


