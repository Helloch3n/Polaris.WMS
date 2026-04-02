using System.Linq;
using Polaris.WMS.Inbound.Application.Contracts.PurchaseOrders;
using Polaris.WMS.Inbound.Application.Contracts.PurchaseOrders.Dtos;
using Polaris.WMS.Inbound.Domain.PurchaseOrders;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Polaris.WMS.Inbound.Application.PurchaseOrders;

public class PurchaseOrderAppService(IRepository<PurchaseOrder, Guid> purchaseOrderRepository)
    : ApplicationService, IPurchaseOrderAppService
{
    public async Task<PurchaseOrderDto> GetAsync(Guid id)
    {
        var queryable = await purchaseOrderRepository.WithDetailsAsync(x => x.Details);
        var entity = await AsyncExecuter.FirstOrDefaultAsync(queryable.Where(x => x.Id == id));

        if (entity == null)
        {
            throw new EntityNotFoundException(typeof(PurchaseOrder), id);
        }

        return ObjectMapper.Map<PurchaseOrder, PurchaseOrderDto>(entity);
    }

    public async Task<PurchaseOrderDto> GetByPoNoAsync(string poNo)
    {
        var queryable = await purchaseOrderRepository.WithDetailsAsync(x => x.Details);
        var entity = await AsyncExecuter.FirstOrDefaultAsync(queryable.Where(x => x.PoNo == poNo));

        if (entity == null)
        {
            throw new UserFriendlyException($"未找到采购订单号为 {poNo} 的单据。");
        }

        return ObjectMapper.Map<PurchaseOrder, PurchaseOrderDto>(entity);
    }

    public async Task<PagedResultDto<PurchaseOrderDto>> GetListAsync(PurchaseOrderSearchDto input)
    {
        input ??= new PurchaseOrderSearchDto();
        var poNo = input.PoNo?.Trim();
        var supplierName = input.SupplierName?.Trim();

        IQueryable<PurchaseOrder> query = await purchaseOrderRepository.GetQueryableAsync();

        if (!string.IsNullOrWhiteSpace(poNo))
        {
            query = query.Where(x => x.PoNo == poNo);
        }

        if (!string.IsNullOrWhiteSpace(supplierName))
        {
            query = query.Where(x => x.SupplierName.Contains(supplierName));
        }

        if (input.Status.HasValue)
        {
            query = query.Where(x => x.Status == input.Status.Value);
        }

        var totalCount = await AsyncExecuter.CountAsync<PurchaseOrder>(query);

        var sortingField = input.Sorting?.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        var isDesc = input.Sorting?.Contains("DESC", StringComparison.OrdinalIgnoreCase) == true;

        query = sortingField switch
        {
            nameof(PurchaseOrder.PoNo) => isDesc ? query.OrderByDescending(x => x.PoNo) : query.OrderBy(x => x.PoNo),
            nameof(PurchaseOrder.SupplierName) => isDesc ? query.OrderByDescending(x => x.SupplierName) : query.OrderBy(x => x.SupplierName),
            nameof(PurchaseOrder.Status) => isDesc ? query.OrderByDescending(x => x.Status) : query.OrderBy(x => x.Status),
            nameof(PurchaseOrder.OrderDate) => isDesc ? query.OrderByDescending(x => x.OrderDate) : query.OrderBy(x => x.OrderDate),
            nameof(PurchaseOrder.CreationTime) => isDesc ? query.OrderByDescending(x => x.CreationTime) : query.OrderBy(x => x.CreationTime),
            _ => query.OrderByDescending(x => x.CreationTime)
        };

        var skipCount = input.SkipCount < 0 ? 0 : input.SkipCount;
        var maxResultCount = input.MaxResultCount <= 0 ? 10 : input.MaxResultCount;

        var items = await AsyncExecuter.ToListAsync<PurchaseOrder>(
            query.Skip(skipCount)
                .Take(maxResultCount)
        );

        return new PagedResultDto<PurchaseOrderDto>(
            totalCount,
            ObjectMapper.Map<List<PurchaseOrder>, List<PurchaseOrderDto>>(items)
        );
    }
}