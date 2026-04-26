using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace Polaris.WMS.Outbound.Domain.SalesOrders;

/// <summary>
/// 销售订单聚合根。
/// 负责表达客户需求与履约进度，不直接执行库存扣减。
/// </summary>
public class SalesOrder : FullAuditedAggregateRoot<Guid>
{
    public string OrderNo { get; private set; }
    public Guid CustomerId { get; private set; }
    public string CustomerCode { get; private set; }
    public string CustomerName { get; private set; }
    public SalesOrderStatus Status { get; private set; }
    public DateTime OrderDate { get; private set; }
    public DateTime? ExpectedDeliveryTime { get; private set; }
    public string? Remark { get; private set; }

    private readonly List<SalesOrderDetail> _details = new();
    public IReadOnlyCollection<SalesOrderDetail> Details => _details;

    protected SalesOrder()
    {
        OrderNo = string.Empty;
        CustomerCode = string.Empty;
        CustomerName = string.Empty;
        Status = SalesOrderStatus.Open;
    }

    internal SalesOrder(
        Guid id,
        string orderNo,
        Guid customerId,
        string customerCode,
        string customerName,
        DateTime orderDate,
        DateTime? expectedDeliveryTime = null,
        string? remark = null) : base(id)
    {
        OrderNo = Check.NotNullOrWhiteSpace(orderNo, nameof(orderNo), maxLength: 64);
        CustomerId = customerId != Guid.Empty ? customerId : throw new BusinessException("客户不能为空。");
        CustomerCode = Check.NotNullOrWhiteSpace(customerCode, nameof(customerCode), maxLength: 50);
        CustomerName = Check.NotNullOrWhiteSpace(customerName, nameof(customerName), maxLength: 200);
        OrderDate = orderDate;
        ExpectedDeliveryTime = expectedDeliveryTime;
        Remark = remark?.Trim();
        Status = SalesOrderStatus.Open;
    }

    public static SalesOrder Create(
        Guid id,
        string orderNo,
        Guid customerId,
        string customerCode,
        string customerName,
        DateTime orderDate,
        DateTime? expectedDeliveryTime = null,
        string? remark = null)
    {
        return new SalesOrder(id, orderNo, customerId, customerCode, customerName, orderDate, expectedDeliveryTime, remark);
    }

    public void UpdateHeader(
        Guid customerId,
        string customerCode,
        string customerName,
        DateTime orderDate,
        DateTime? expectedDeliveryTime,
        string? remark)
    {
        EnsureEditable();
        CustomerId = customerId != Guid.Empty ? customerId : throw new BusinessException("客户不能为空。");
        CustomerCode = Check.NotNullOrWhiteSpace(customerCode, nameof(customerCode), maxLength: 50);
        CustomerName = Check.NotNullOrWhiteSpace(customerName, nameof(customerName), maxLength: 200);
        OrderDate = orderDate;
        ExpectedDeliveryTime = expectedDeliveryTime;
        Remark = remark?.Trim();
    }

    public SalesOrderDetail AddDetail(
        Guid detailId,
        int lineNo,
        Guid productId,
        string productCode,
        string productName,
        string unit,
        decimal qty,
        string? remark = null)
    {
        EnsureEditable();

        if (_details.Any(x => x.LineNo == lineNo))
        {
            throw new BusinessException("销售订单已存在重复行号。")
                .WithData("LineNo", lineNo);
        }

        var detail = new SalesOrderDetail(detailId, Id, lineNo, productId, productCode, productName, unit, qty, remark);
        _details.Add(detail);
        return detail;
    }

    public void RemoveDetail(Guid detailId)
    {
        EnsureEditable();

        var detail = _details.FirstOrDefault(x => x.Id == detailId);
        if (detail != null)
        {
            _details.Remove(detail);
        }
    }

    public void ApplyShipment(Guid detailId, decimal qty)
    {
        var detail = _details.FirstOrDefault(x => x.Id == detailId);
        if (detail == null)
        {
            throw new BusinessException("销售订单明细不存在。")
                .WithData("DetailId", detailId);
        }

        detail.AddShippedQty(qty);
        RefreshStatus();
    }

    public void Cancel()
    {
        if (_details.Any(x => x.ShippedQty > 0))
        {
            throw new BusinessException("销售订单已存在发货数量，不能取消。")
                .WithData("OrderNo", OrderNo);
        }

        Status = SalesOrderStatus.Cancelled;
    }

    public void RefreshStatus()
    {
        if (Status == SalesOrderStatus.Cancelled)
        {
            return;
        }

        if (!_details.Any())
        {
            Status = SalesOrderStatus.Open;
            return;
        }

        if (_details.All(x => x.ShippedQty >= x.Qty))
        {
            Status = SalesOrderStatus.Completed;
            return;
        }

        if (_details.Any(x => x.ShippedQty > 0))
        {
            Status = SalesOrderStatus.PartiallyShipped;
            return;
        }

        Status = SalesOrderStatus.Open;
    }

    private void EnsureEditable()
    {
        if (Status == SalesOrderStatus.Completed || Status == SalesOrderStatus.Cancelled)
        {
            throw new BusinessException("当前销售订单状态不允许修改。")
                .WithData("Status", Status);
        }
    }
}

public class SalesOrderDetail : Entity<Guid>
{
    public Guid SalesOrderId { get; private set; }
    public int LineNo { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductCode { get; private set; }
    public string ProductName { get; private set; }
    public string Unit { get; private set; }
    public decimal Qty { get; private set; }
    public decimal AllocatedQty { get; private set; }
    public decimal ShippedQty { get; private set; }
    public string? Remark { get; private set; }

    protected SalesOrderDetail()
    {
        ProductCode = string.Empty;
        ProductName = string.Empty;
        Unit = string.Empty;
    }

    internal SalesOrderDetail(
        Guid id,
        Guid salesOrderId,
        int lineNo,
        Guid productId,
        string productCode,
        string productName,
        string unit,
        decimal qty,
        string? remark = null) : base(id)
    {
        SalesOrderId = salesOrderId;
        LineNo = lineNo;
        ProductId = productId != Guid.Empty ? productId : throw new BusinessException("物料不能为空。");
        ProductCode = Check.NotNullOrWhiteSpace(productCode, nameof(productCode), maxLength: 50);
        ProductName = Check.NotNullOrWhiteSpace(productName, nameof(productName), maxLength: 200);
        Unit = Check.NotNullOrWhiteSpace(unit, nameof(unit), maxLength: 20);
        Qty = qty > 0 ? qty : throw new BusinessException("销售订单数量必须大于0。").WithData("Qty", qty);
        AllocatedQty = 0;
        ShippedQty = 0;
        Remark = remark?.Trim();
    }

    public void AddShippedQty(decimal qty)
    {
        if (qty <= 0)
        {
            throw new BusinessException("发货数量必须大于0。")
                .WithData("Qty", qty);
        }

        if (ShippedQty + qty > Qty)
        {
            throw new BusinessException("累计发货数量不能大于订单数量。")
                .WithData("LineNo", LineNo)
                .WithData("Qty", Qty)
                .WithData("ShippedQty", ShippedQty)
                .WithData("IncrementQty", qty);
        }

        ShippedQty += qty;
    }
}

