using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace Polaris.WMS.InventoryManage.Domain.CycleCountOrders;

public class CycleCountOrderDetail : Entity<Guid>
{
    public Guid CycleCountOrderId { get; private set; }
    public Guid LocationId { get; private set; }
    public string ContainerCode { get; private set; }
    public Guid ProductId { get; private set; }

    public decimal SystemQty { get; private set; }
    public decimal? CountedQty { get; private set; }
    public decimal DifferenceQty { get; private set; }

    public bool IsCounted { get; private set; }
    public DateTime? CountedTime { get; private set; }

    public bool IsInventoryAdjusted { get; private set; }

    protected CycleCountOrderDetail()
    {
    }

    internal CycleCountOrderDetail(
        Guid id,
        Guid cycleCountOrderId,
        Guid locationId,
        string containerCode,
        Guid productId,
        decimal systemQty) : base(id)
    {
        if (locationId == Guid.Empty)
        {
            throw new BusinessException("库位不能为空");
        }

        if (productId == Guid.Empty)
        {
            throw new BusinessException("物料不能为空");
        }

        if (systemQty < 0)
        {
            throw new BusinessException("账面数量不能小于0");
        }

        CycleCountOrderId = cycleCountOrderId;
        LocationId = locationId;
        ContainerCode = Check.NotNullOrWhiteSpace(containerCode, nameof(containerCode), 64);
        ProductId = productId;
        SystemQty = systemQty;
        DifferenceQty = 0;
    }

    internal void SubmitCountedQty(decimal countedQty)
    {
        if (countedQty < 0)
        {
            throw new BusinessException("实盘数量不能小于0");
        }

        if (IsCounted)
        {
            throw new BusinessException("该明细已提交实盘结果，不允许重复提交");
        }

        CountedQty = countedQty;
        DifferenceQty = countedQty - SystemQty;
        IsCounted = true;
        CountedTime = DateTime.Now;
    }

    internal void MarkInventoryAdjusted()
    {
        IsInventoryAdjusted = true;
    }
}
