using Polaris.WMS.Isolation;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Polaris.WMS.InventoryManage.Domain.CycleCountOrders;

public class CycleCountOrder : FullAuditedAggregateRoot<Guid>, IMultiTenant, IMultiWarehouse
{
    public Guid? TenantId { get; set; }
    public Guid WarehouseId { get; private set; }
    public string OrderNo { get; private set; }
    public CycleCountType CountType { get; private set; }
    public CycleCountOrderStatus Status { get; private set; }

    private readonly List<CycleCountOrderDetail> _details = new();
    public IReadOnlyCollection<CycleCountOrderDetail> Details => _details;

    protected CycleCountOrder()
    {
    }

    internal CycleCountOrder(
        Guid id,
        Guid? tenantId,
        Guid warehouseId,
        string orderNo,
        CycleCountType countType) : base(id)
    {
        if (warehouseId == Guid.Empty)
        {
            throw new BusinessException("盘点单仓库不能为空");
        }

        TenantId = tenantId;
        WarehouseId = warehouseId;
        OrderNo = Check.NotNullOrWhiteSpace(orderNo, nameof(orderNo), 64);
        CountType = countType;
        Status = CycleCountOrderStatus.New;
    }

    public static CycleCountOrder Create(
        Guid id,
        Guid? tenantId,
        Guid warehouseId,
        string orderNo,
        CycleCountType countType)
    {
        return new CycleCountOrder(id, tenantId, warehouseId, orderNo, countType);
    }

    public CycleCountOrderDetail AddDetail(
        Guid detailId,
        Guid locationId,
        string containerCode,
        Guid productId,
        decimal systemQty)
    {
        EnsureEditable();

        if (_details.Any(x =>
                x.LocationId == locationId &&
                x.ContainerCode == containerCode &&
                x.ProductId == productId))
        {
            throw new BusinessException("盘点明细重复")
                .WithData("ContainerCode", containerCode)
                .WithData("ProductId", productId);
        }

        var detail = new CycleCountOrderDetail(
            detailId,
            Id,
            locationId,
            containerCode,
            productId,
            systemQty);

        _details.Add(detail);
        return detail;
    }

    public CycleCountOrderDetail SubmitCountResult(string containerCode, Guid productId, decimal countedQty)
    {
        if (Status is CycleCountOrderStatus.Completed or CycleCountOrderStatus.Voided)
        {
            throw new BusinessException("当前盘点单状态不允许继续盘点");
        }

        if (Status == CycleCountOrderStatus.New)
        {
            Status = CycleCountOrderStatus.Counting;
        }

        var detail = _details.FirstOrDefault(x =>
            x.ContainerCode == containerCode &&
            x.ProductId == productId);

        if (detail == null)
        {
            throw new BusinessException("未找到对应盘点明细")
                .WithData("ContainerCode", containerCode)
                .WithData("ProductId", productId);
        }

        detail.SubmitCountedQty(countedQty);
        return detail;
    }

    public bool CanComplete()
    {
        return _details.Count > 0 && _details.All(x => x.IsCounted);
    }

    public void Complete()
    {
        if (!CanComplete())
        {
            throw new BusinessException("存在未盘点明细，不能完成盘点单");
        }

        Status = CycleCountOrderStatus.Completed;
    }

    public void Void()
    {
        if (Status == CycleCountOrderStatus.Completed)
        {
            throw new BusinessException("已完成盘点单不允许作废");
        }

        Status = CycleCountOrderStatus.Voided;
    }

    private void EnsureEditable()
    {
        if (Status is CycleCountOrderStatus.Completed or CycleCountOrderStatus.Voided)
        {
            throw new BusinessException("当前盘点单状态不允许修改");
        }
    }
}

public enum CycleCountType
{
    Open = 1,
    Blind = 2
}

public enum CycleCountOrderStatus
{
    New = 0,
    Counting = 1,
    Completed = 2,
    Voided = 3
}
