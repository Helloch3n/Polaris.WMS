using Polaris.WMS.MasterData.Application.Contracts.Integration.Customers;
using Polaris.WMS.Outbound.Application.Contracts.SalesOrders;
using Polaris.WMS.Outbound.Domain.SalesOrders;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Polaris.WMS.Outbound.Application.SalesOrders;

public class SalesOrderAppService(
    IRepository<SalesOrder, Guid> salesOrderRepository,
    ICustomerIntegrationService customerIntegrationService)
    : ApplicationService, ISalesOrderAppService
{
    /// <inheritdoc />
    public async Task<SalesOrderDto> CreateAsync(CreateSalesOrderDto input)
    {
        var customer = await customerIntegrationService.GetAsync(input.CustomerId);
        if (!customer.IsEnabled)
        {
            throw new BusinessException("客户已停用，不能创建销售订单。")
                .WithData("CustomerId", input.CustomerId);
        }

        var exists = await salesOrderRepository.AnyAsync(x => x.OrderNo == input.OrderNo);
        if (exists)
        {
            throw new BusinessException("销售订单号已存在。")
                .WithData("OrderNo", input.OrderNo);
        }

        var entity = SalesOrder.Create(
            GuidGenerator.Create(),
            input.OrderNo,
            customer.Id,
            customer.Code,
            customer.Name,
            input.OrderDate,
            input.ExpectedDeliveryTime,
            input.Remark);

        foreach (var detail in input.Details)
        {
            entity.AddDetail(
                GuidGenerator.Create(),
                detail.LineNo,
                detail.ProductId,
                detail.ProductCode,
                detail.ProductName,
                detail.Unit,
                detail.Qty,
                detail.Remark);
        }

        await salesOrderRepository.InsertAsync(entity);
        return MapToDto(entity);
    }

    /// <inheritdoc />
    public async Task<SalesOrderDto> UpdateAsync(Guid id, UpdateSalesOrderDto input)
    {
        var query = await salesOrderRepository.WithDetailsAsync(x => x.Details);
        var entity = await AsyncExecuter.FirstOrDefaultAsync(query.Where(x => x.Id == id));
        if (entity == null)
        {
            throw new EntityNotFoundException(typeof(SalesOrder), id);
        }

        var customer = await customerIntegrationService.GetAsync(input.CustomerId);
        if (!customer.IsEnabled)
        {
            throw new BusinessException("客户已停用，不能更新销售订单。")
                .WithData("CustomerId", input.CustomerId);
        }

        entity.UpdateHeader(customer.Id, customer.Code, customer.Name, input.OrderDate, input.ExpectedDeliveryTime, input.Remark);

        foreach (var detail in entity.Details.ToList())
        {
            entity.RemoveDetail(detail.Id);
        }

        foreach (var detail in input.Details)
        {
            entity.AddDetail(
                GuidGenerator.Create(),
                detail.LineNo,
                detail.ProductId,
                detail.ProductCode,
                detail.ProductName,
                detail.Unit,
                detail.Qty,
                detail.Remark);
        }

        await salesOrderRepository.UpdateAsync(entity);
        return MapToDto(entity);
    }

    /// <inheritdoc />
    public async Task<SalesOrderDto> GetAsync(Guid id)
    {
        var query = await salesOrderRepository.WithDetailsAsync(x => x.Details);
        var entity = await AsyncExecuter.FirstOrDefaultAsync(query.Where(x => x.Id == id));
        if (entity == null)
        {
            throw new EntityNotFoundException(typeof(SalesOrder), id);
        }

        return MapToDto(entity);
    }

    /// <inheritdoc />
    public async Task<PagedResultDto<SalesOrderDto>> GetListAsync(SalesOrderSearchDto input)
    {
        var query = await salesOrderRepository.GetQueryableAsync();

        if (!string.IsNullOrWhiteSpace(input.OrderNo))
        {
            query = query.Where(x => x.OrderNo.Contains(input.OrderNo));
        }

        if (!string.IsNullOrWhiteSpace(input.CustomerCode))
        {
            query = query.Where(x => x.CustomerCode.Contains(input.CustomerCode));
        }

        if (input.Status.HasValue)
        {
            query = query.Where(x => x.Status == input.Status.Value);
        }

        var totalCount = await AsyncExecuter.CountAsync(query);
        query = query.OrderByDescending(x => x.CreationTime).PageBy(input.SkipCount, input.MaxResultCount);
        var entities = await AsyncExecuter.ToListAsync(query);
        var items = entities.Select(MapToDtoWithoutDetails).ToList();
        return new PagedResultDto<SalesOrderDto>(totalCount, items);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id)
    {
        await salesOrderRepository.DeleteAsync(id);
    }

    private static SalesOrderDto MapToDto(SalesOrder source)
    {
        var dto = MapToDtoWithoutDetails(source);
        dto.Details = source.Details.Select(x => new SalesOrderDetailDto
        {
            Id = x.Id,
            SalesOrderId = x.SalesOrderId,
            LineNo = x.LineNo,
            ProductId = x.ProductId,
            ProductCode = x.ProductCode,
            ProductName = x.ProductName,
            Unit = x.Unit,
            Qty = x.Qty,
            AllocatedQty = x.AllocatedQty,
            ShippedQty = x.ShippedQty,
            Remark = x.Remark
        }).ToList();
        return dto;
    }

    private static SalesOrderDto MapToDtoWithoutDetails(SalesOrder source)
    {
        return new SalesOrderDto
        {
            Id = source.Id,
            OrderNo = source.OrderNo,
            CustomerId = source.CustomerId,
            CustomerCode = source.CustomerCode,
            CustomerName = source.CustomerName,
            Status = source.Status,
            OrderDate = source.OrderDate,
            ExpectedDeliveryTime = source.ExpectedDeliveryTime,
            Remark = source.Remark,
            CreationTime = source.CreationTime,
            CreatorId = source.CreatorId,
            LastModificationTime = source.LastModificationTime,
            LastModifierId = source.LastModifierId,
            Details = new List<SalesOrderDetailDto>()
        };
    }
}


