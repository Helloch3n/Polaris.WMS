using System.Linq.Dynamic.Core;
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

    public async Task<PagedResultDto<PurchaseOrderDto>> GetListAsync(GetPurchaseOrderListDto input)
    {
        var query = (await purchaseOrderRepository.GetQueryableAsync())
            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                x => x.PoNo.Contains(input.Filter) || x.SupplierName.Contains(input.Filter))
            .WhereIf(input.Status.HasValue, x => x.Status == input.Status);

        var totalCount = await AsyncExecuter.CountAsync(query);

        var items = await AsyncExecuter.ToListAsync(
            query.OrderBy(string.IsNullOrWhiteSpace(input.Sorting) ? "CreationTime DESC" : input.Sorting)
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
        );

        return new PagedResultDto<PurchaseOrderDto>(
            totalCount,
            ObjectMapper.Map<List<PurchaseOrder>, List<PurchaseOrderDto>>(items)
        );
    }
}