using Polaris.WMS.Outbound.Application.Contracts.Handovers;
using Polaris.WMS.Outbound.Domain;
using Polaris.WMS.Outbound.Domain.Handovers;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Polaris.WMS.Outbound.Application.Handovers;

public class OutboundHandoverOrderAppService(
    IRepository<OutboundHandoverOrder, Guid> outboundHandoverOrderRepository,
    OutboundFulfillmentManager outboundFulfillmentManager)
    : ApplicationService, IOutboundHandoverOrderAppService
{
    /// <inheritdoc />
    public async Task<OutboundHandoverOrderDto> CreateAsync(CreateOutboundHandoverOrderDto input)
    {
        var entity = await outboundFulfillmentManager.CreateHandoverAsync(input.OutboundReviewOrderId, input.Remark);
        return MapToDto(entity);
    }

    /// <inheritdoc />
    public async Task CompleteAsync(Guid id)
    {
        await outboundFulfillmentManager.CompleteHandoverAsync(id);
    }

    /// <inheritdoc />
    public async Task<OutboundHandoverOrderDto> GetAsync(Guid id)
    {
        var query = await outboundHandoverOrderRepository.WithDetailsAsync(x => x.Lines);
        var entity = await AsyncExecuter.FirstOrDefaultAsync(query.Where(x => x.Id == id));
        if (entity == null)
        {
            throw new EntityNotFoundException(typeof(OutboundHandoverOrder), id);
        }

        return MapToDto(entity);
    }

    /// <inheritdoc />
    public async Task<PagedResultDto<OutboundHandoverOrderDto>> GetListAsync(OutboundHandoverOrderSearchDto input)
    {
        var query = await outboundHandoverOrderRepository.GetQueryableAsync();

        if (!string.IsNullOrWhiteSpace(input.HandoverNo))
        {
            query = query.Where(x => x.HandoverNo.Contains(input.HandoverNo));
        }

        if (input.Status.HasValue)
        {
            query = query.Where(x => x.Status == input.Status.Value);
        }

        if (input.OutboundReviewOrderId.HasValue)
        {
            query = query.Where(x => x.OutboundReviewOrderId == input.OutboundReviewOrderId.Value);
        }

        var totalCount = await AsyncExecuter.CountAsync(query);
        var items = await AsyncExecuter.ToListAsync(query.OrderByDescending(x => x.CreationTime).PageBy(input.SkipCount, input.MaxResultCount));
        return new PagedResultDto<OutboundHandoverOrderDto>(totalCount, items.Select(MapToListDto).ToList());
    }

    private static OutboundHandoverOrderDto MapToDto(OutboundHandoverOrder source)
    {
        return new OutboundHandoverOrderDto
        {
            Id = source.Id,
            HandoverNo = source.HandoverNo,
            OutboundReviewOrderId = source.OutboundReviewOrderId,
            Status = source.Status,
            Remark = source.Remark,
            CreationTime = source.CreationTime,
            CreatorId = source.CreatorId,
            LastModificationTime = source.LastModificationTime,
            LastModifierId = source.LastModifierId,
            Lines = source.Lines.Select(x => new OutboundHandoverLineDto
            {
                Id = x.Id,
                OutboundHandoverOrderId = x.OutboundHandoverOrderId,
                ReviewLineId = x.ReviewLineId,
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

    private static OutboundHandoverOrderDto MapToListDto(OutboundHandoverOrder source)
    {
        return new OutboundHandoverOrderDto
        {
            Id = source.Id,
            HandoverNo = source.HandoverNo,
            OutboundReviewOrderId = source.OutboundReviewOrderId,
            Status = source.Status,
            Remark = source.Remark,
            CreationTime = source.CreationTime,
            CreatorId = source.CreatorId,
            LastModificationTime = source.LastModificationTime,
            LastModifierId = source.LastModifierId,
            Lines = new List<OutboundHandoverLineDto>()
        };
    }
}

