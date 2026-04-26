using Polaris.WMS.Outbound.Application.Contracts.Reviews;
using Polaris.WMS.Outbound.Domain;
using Polaris.WMS.Outbound.Domain.Reviews;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Polaris.WMS.Outbound.Application.Reviews;

public class OutboundReviewOrderAppService(
    IRepository<OutboundReviewOrder, Guid> outboundReviewOrderRepository,
    OutboundFulfillmentManager outboundFulfillmentManager)
    : ApplicationService, IOutboundReviewOrderAppService
{
    /// <inheritdoc />
    public async Task<OutboundReviewOrderDto> CreateAsync(CreateOutboundReviewOrderDto input)
    {
        var entity = await outboundFulfillmentManager.CreateReviewAsync(input.PickListId, input.Remark);
        return MapToDto(entity);
    }

    /// <inheritdoc />
    public async Task CompleteAsync(Guid id)
    {
        await outboundFulfillmentManager.CompleteReviewAsync(id);
    }

    /// <inheritdoc />
    public async Task<OutboundReviewOrderDto> GetAsync(Guid id)
    {
        var query = await outboundReviewOrderRepository.WithDetailsAsync(x => x.Lines);
        var entity = await AsyncExecuter.FirstOrDefaultAsync(query.Where(x => x.Id == id));
        if (entity == null)
        {
            throw new EntityNotFoundException(typeof(OutboundReviewOrder), id);
        }

        return MapToDto(entity);
    }

    /// <inheritdoc />
    public async Task<PagedResultDto<OutboundReviewOrderDto>> GetListAsync(OutboundReviewOrderSearchDto input)
    {
        var query = await outboundReviewOrderRepository.GetQueryableAsync();

        if (!string.IsNullOrWhiteSpace(input.ReviewNo))
        {
            query = query.Where(x => x.ReviewNo.Contains(input.ReviewNo));
        }

        if (input.Status.HasValue)
        {
            query = query.Where(x => x.Status == input.Status.Value);
        }

        if (input.PickListId.HasValue)
        {
            query = query.Where(x => x.PickListId == input.PickListId.Value);
        }

        var totalCount = await AsyncExecuter.CountAsync(query);
        var items = await AsyncExecuter.ToListAsync(query.OrderByDescending(x => x.CreationTime).PageBy(input.SkipCount, input.MaxResultCount));
        return new PagedResultDto<OutboundReviewOrderDto>(totalCount, items.Select(MapToListDto).ToList());
    }

    private static OutboundReviewOrderDto MapToDto(OutboundReviewOrder source)
    {
        return new OutboundReviewOrderDto
        {
            Id = source.Id,
            ReviewNo = source.ReviewNo,
            PickListId = source.PickListId,
            Status = source.Status,
            Remark = source.Remark,
            CreationTime = source.CreationTime,
            CreatorId = source.CreatorId,
            LastModificationTime = source.LastModificationTime,
            LastModifierId = source.LastModifierId,
            Lines = source.Lines.Select(x => new OutboundReviewLineDto
            {
                Id = x.Id,
                OutboundReviewOrderId = x.OutboundReviewOrderId,
                PickListLineId = x.PickListLineId,
                SalesShipmentId = x.SalesShipmentId,
                SalesShipmentDetailId = x.SalesShipmentDetailId,
                SalesShipmentAllocationId = x.SalesShipmentAllocationId,
                ProductId = x.ProductId,
                ProductCode = x.ProductCode,
                ProductName = x.ProductName,
                Qty = x.Qty,
                ContainerId = x.ContainerId,
                ContainerCode = x.ContainerCode,
                LocationId = x.LocationId,
                LocationCode = x.LocationCode,
                BatchNo = x.BatchNo,
                SN = x.SN
            }).ToList()
        };
    }

    private static OutboundReviewOrderDto MapToListDto(OutboundReviewOrder source)
    {
        return new OutboundReviewOrderDto
        {
            Id = source.Id,
            ReviewNo = source.ReviewNo,
            PickListId = source.PickListId,
            Status = source.Status,
            Remark = source.Remark,
            CreationTime = source.CreationTime,
            CreatorId = source.CreatorId,
            LastModificationTime = source.LastModificationTime,
            LastModifierId = source.LastModifierId,
            Lines = new List<OutboundReviewLineDto>()
        };
    }
}

