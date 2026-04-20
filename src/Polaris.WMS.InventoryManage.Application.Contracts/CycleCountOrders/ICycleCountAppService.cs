using Polaris.WMS.InventoryManage.Application.Contracts.CycleCountOrders.Dtos;
using Volo.Abp.Application.Services;

namespace Polaris.WMS.InventoryManage.Application.Contracts.CycleCountOrders;

public interface ICycleCountAppService : IApplicationService
{
    Task SubmitCountResultAsync(SubmitCountResultInput input);
}