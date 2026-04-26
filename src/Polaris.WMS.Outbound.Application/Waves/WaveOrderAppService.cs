using Polaris.WMS.Outbound.Application.Contracts.Waves;
using Polaris.WMS.Outbound.Domain;
using Polaris.WMS.Outbound.Domain.WaveOrders;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Polaris.WMS.Outbound.Application.Waves;

public class WaveOrderAppService(
    IRepository<WaveOrder, Guid> waveOrderRepository,
    OutboundFulfillmentManager outboundFulfillmentManager)
    : ApplicationService, IWaveOrderAppService
{
    /// <inheritdoc />
    public async Task<WaveOrderDto> CreateAsync(CreateWaveOrderDto input)
    {
        var wave = await outboundFulfillmentManager.CreateWaveAsync(input.SalesShipmentIds, input.Remark);
        return MapToDto(wave);
    }

    /// <inheritdoc />
    public async Task<WaveOrderDto> GetAsync(Guid id)
    {
        var query = await waveOrderRepository.WithDetailsAsync(x => x.Lines);
        var entity = await AsyncExecuter.FirstOrDefaultAsync(query.Where(x => x.Id == id));
        if (entity == null)
        {
            throw new EntityNotFoundException(typeof(WaveOrder), id);
        }

        return MapToDto(entity);
    }

    /// <inheritdoc />
    public async Task<PagedResultDto<WaveOrderDto>> GetListAsync(WaveOrderSearchDto input)
    {
        var query = await waveOrderRepository.GetQueryableAsync();

        if (!string.IsNullOrWhiteSpace(input.WaveNo))
        {
            query = query.Where(x => x.WaveNo.Contains(input.WaveNo));
        }

        if (input.Status.HasValue)
        {
            query = query.Where(x => x.Status == input.Status.Value);
        }

        var totalCount = await AsyncExecuter.CountAsync(query);
        var items = await AsyncExecuter.ToListAsync(query.OrderByDescending(x => x.CreationTime).PageBy(input.SkipCount, input.MaxResultCount));
        return new PagedResultDto<WaveOrderDto>(totalCount, items.Select(MapToListDto).ToList());
    }

    private static WaveOrderDto MapToDto(WaveOrder source)
    {
        return new WaveOrderDto
        {
            Id = source.Id,
            WaveNo = source.WaveNo,
            Status = source.Status,
            Remark = source.Remark,
            CreationTime = source.CreationTime,
            CreatorId = source.CreatorId,
            LastModificationTime = source.LastModificationTime,
            LastModifierId = source.LastModifierId,
            Lines = source.Lines.Select(x => new WaveOrderLineDto
            {
                Id = x.Id,
                WaveOrderId = x.WaveOrderId,
                SalesShipmentId = x.SalesShipmentId,
                SalesShipmentNo = x.SalesShipmentNo,
                SalesShipmentDetailId = x.SalesShipmentDetailId,
                CustomerId = x.CustomerId,
                CustomerCode = x.CustomerCode,
                CustomerName = x.CustomerName,
                ProductId = x.ProductId,
                ProductCode = x.ProductCode,
                ProductName = x.ProductName,
                Qty = x.Qty
            }).ToList()
        };
    }

    private static WaveOrderDto MapToListDto(WaveOrder source)
    {
        return new WaveOrderDto
        {
            Id = source.Id,
            WaveNo = source.WaveNo,
            Status = source.Status,
            Remark = source.Remark,
            CreationTime = source.CreationTime,
            CreatorId = source.CreatorId,
            LastModificationTime = source.LastModificationTime,
            LastModifierId = source.LastModifierId,
            Lines = new List<WaveOrderLineDto>()
        };
    }
}

