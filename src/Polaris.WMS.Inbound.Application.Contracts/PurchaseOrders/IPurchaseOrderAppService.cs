using Polaris.WMS.Inbound.Application.Contracts.PurchaseOrders.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Polaris.WMS.Inbound.Application.Contracts.PurchaseOrders;

public interface IPurchaseOrderAppService : IApplicationService
{
    Task<PurchaseOrderDto> GetAsync(Guid id);
    Task<PurchaseOrderDto> GetByPoNoAsync(string poNo);
    Task<PagedResultDto<PurchaseOrderDto>> GetListAsync(PurchaseOrderSearchDto input);
}