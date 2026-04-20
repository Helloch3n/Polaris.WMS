using Polaris.WMS.InventoryManage.Application.Contracts.CycleCountOrders.Dtos;
using Polaris.WMS.InventoryManage.Domain.CycleCountOrders;
using Polaris.WMS.Isolation;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Polaris.WMS.InventoryManage.Application.CycleCountOrders;

public class CycleCountAppService(
    IRepository<CycleCountOrder, Guid> cycleCountOrderRepository,
    CycleCountOrderManager cycleCountOrderManager,
    IWMSContextProvider wmsContextProvider)
    : ApplicationService
{
    public async Task SubmitCountResultAsync(SubmitCountResultInput input)
    {
        if (input.OrderId == Guid.Empty)
        {
            throw new BusinessException("盘点单Id不能为空");
        }

        if (input.ProductId == Guid.Empty)
        {
            throw new BusinessException("物料Id不能为空");
        }

        if (string.IsNullOrWhiteSpace(input.ContainerCode))
        {
            throw new BusinessException("托盘码不能为空");
        }

        if (input.CountedQty < 0)
        {
            throw new BusinessException("实盘数量不能小于0");
        }

        var query = await cycleCountOrderRepository.WithDetailsAsync(x => x.Details);
        var order = await AsyncExecuter.FirstOrDefaultAsync(query.Where(x => x.Id == input.OrderId));
        if (order == null)
        {
            throw new BusinessException("盘点单不存在").WithData("OrderId", input.OrderId);
        }

        if (CurrentTenant.Id.HasValue && order.TenantId.HasValue && CurrentTenant.Id.Value != order.TenantId.Value)
        {
            throw new UserFriendlyException("非法操作：租户隔离校验不通过。");
        }

        if (wmsContextProvider.CurrentWarehouseId.HasValue &&
            wmsContextProvider.CurrentWarehouseId.Value != order.WarehouseId)
        {
            throw new UserFriendlyException("非法操作：仓库隔离校验不通过。");
        }

        await cycleCountOrderManager.SubmitCountResultAsync(
            order,
            input.ContainerCode.Trim(),
            input.ProductId,
            input.CountedQty);

        await cycleCountOrderRepository.UpdateAsync(order, autoSave: true);
    }
}