using Polaris.WMS.Inbound.Application.Contracts.PurchaseOrders.Dtos;
using Polaris.WMS.Inbound.Application.Contracts.DataSync.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Polaris.WMS.Inbound.Application.Contracts.PurchaseOrders;

public interface IPurchaseOrderAppService : IApplicationService
{
    /// <summary>
    /// 同步 (Upsert) 采购订单。
    /// </summary>
    Task SyncPoAsync(SyncPoDto input);

    Task<PurchaseOrderDto> GetAsync(Guid id);
    Task<PurchaseOrderDto> GetByPoNoAsync(string poNo);
    Task<PagedResultDto<PurchaseOrderDto>> GetListAsync(PurchaseOrderSearchDto input);
}