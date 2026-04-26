using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace Polaris.WMS.Outbound.Domain.WaveOrders;

/// <summary>
/// 出库波次单聚合根。
/// </summary>
public class WaveOrder : FullAuditedAggregateRoot<Guid>
{
    public string WaveNo { get; private set; }

    public WaveOrderStatus Status { get; private set; }

    public string? Remark { get; private set; }

    private readonly List<WaveOrderLine> _lines = new();
    public IReadOnlyCollection<WaveOrderLine> Lines => _lines;

    protected WaveOrder()
    {
        WaveNo = string.Empty;
        Status = WaveOrderStatus.Created;
    }

    internal WaveOrder(Guid id, string waveNo, string? remark = null)
        : base(id)
    {
        WaveNo = Check.NotNullOrWhiteSpace(waveNo, nameof(waveNo), maxLength: 64);
        Remark = remark?.Trim();
        Status = WaveOrderStatus.Created;
    }

    public static WaveOrder Create(Guid id, string waveNo, string? remark = null)
    {
        return new WaveOrder(id, waveNo, remark);
    }

    public WaveOrderLine AddLine(
        Guid lineId,
        Guid salesShipmentId,
        string salesShipmentNo,
        Guid salesShipmentDetailId,
        Guid customerId,
        string customerCode,
        string customerName,
        Guid productId,
        string productCode,
        string productName,
        decimal qty)
    {
        EnsureEditable();

        var line = new WaveOrderLine(
            lineId,
            Id,
            salesShipmentId,
            salesShipmentNo,
            salesShipmentDetailId,
            customerId,
            customerCode,
            customerName,
            productId,
            productCode,
            productName,
            qty);

        _lines.Add(line);
        return line;
    }

    public void Release()
    {
        EnsureEditable();

        if (!_lines.Any())
        {
            throw new BusinessException("波次单至少需要一条明细。");
        }

        Status = WaveOrderStatus.Released;
    }

    public void Complete()
    {
        if (!_lines.Any())
        {
            throw new BusinessException("波次单至少需要一条明细。");
        }

        Status = WaveOrderStatus.Completed;
    }

    public void Cancel()
    {
        if (Status == WaveOrderStatus.Completed)
        {
            throw new BusinessException("已完成的波次单不允许取消。")
                .WithData("Status", Status);
        }

        Status = WaveOrderStatus.Cancelled;
    }

    private void EnsureEditable()
    {
        if (Status != WaveOrderStatus.Created)
        {
            throw new BusinessException("仅新建状态的波次单允许修改。")
                .WithData("Status", Status);
        }
    }
}

public class WaveOrderLine : Entity<Guid>
{
    public Guid WaveOrderId { get; private set; }

    public Guid SalesShipmentId { get; private set; }

    public string SalesShipmentNo { get; private set; }

    public Guid SalesShipmentDetailId { get; private set; }

    public Guid CustomerId { get; private set; }

    public string CustomerCode { get; private set; }

    public string CustomerName { get; private set; }

    public Guid ProductId { get; private set; }

    public string ProductCode { get; private set; }

    public string ProductName { get; private set; }

    public decimal Qty { get; private set; }

    protected WaveOrderLine()
    {
        SalesShipmentNo = string.Empty;
        CustomerCode = string.Empty;
        CustomerName = string.Empty;
        ProductCode = string.Empty;
        ProductName = string.Empty;
    }

    internal WaveOrderLine(
        Guid id,
        Guid waveOrderId,
        Guid salesShipmentId,
        string salesShipmentNo,
        Guid salesShipmentDetailId,
        Guid customerId,
        string customerCode,
        string customerName,
        Guid productId,
        string productCode,
        string productName,
        decimal qty)
        : base(id)
    {
        WaveOrderId = waveOrderId != Guid.Empty ? waveOrderId : throw new BusinessException("波次单Id不能为空。");
        SalesShipmentId = salesShipmentId != Guid.Empty ? salesShipmentId : throw new BusinessException("销售发货单Id不能为空。");
        SalesShipmentNo = Check.NotNullOrWhiteSpace(salesShipmentNo, nameof(salesShipmentNo), maxLength: 64);
        SalesShipmentDetailId = salesShipmentDetailId != Guid.Empty ? salesShipmentDetailId : throw new BusinessException("销售发货明细Id不能为空。");
        CustomerId = customerId != Guid.Empty ? customerId : throw new BusinessException("客户Id不能为空。");
        CustomerCode = Check.NotNullOrWhiteSpace(customerCode, nameof(customerCode), maxLength: 50);
        CustomerName = Check.NotNullOrWhiteSpace(customerName, nameof(customerName), maxLength: 200);
        ProductId = productId != Guid.Empty ? productId : throw new BusinessException("物料Id不能为空。");
        ProductCode = Check.NotNullOrWhiteSpace(productCode, nameof(productCode), maxLength: 50);
        ProductName = Check.NotNullOrWhiteSpace(productName, nameof(productName), maxLength: 200);
        Qty = qty > 0 ? qty : throw new BusinessException("波次数量必须大于0。");
    }
}

