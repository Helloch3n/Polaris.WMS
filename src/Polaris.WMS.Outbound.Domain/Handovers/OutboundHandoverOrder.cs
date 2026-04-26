using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace Polaris.WMS.Outbound.Domain.Handovers;

/// <summary>
/// 出库交接单聚合根。
/// </summary>
public class OutboundHandoverOrder : FullAuditedAggregateRoot<Guid>
{
    public string HandoverNo { get; private set; }

    public Guid OutboundReviewOrderId { get; private set; }

    public OutboundHandoverStatus Status { get; private set; }

    public string? Remark { get; private set; }

    private readonly List<OutboundHandoverLine> _lines = new();
    public IReadOnlyCollection<OutboundHandoverLine> Lines => _lines;

    protected OutboundHandoverOrder()
    {
        HandoverNo = string.Empty;
        Status = OutboundHandoverStatus.Created;
    }

    internal OutboundHandoverOrder(Guid id, string handoverNo, Guid outboundReviewOrderId, string? remark = null)
        : base(id)
    {
        HandoverNo = Check.NotNullOrWhiteSpace(handoverNo, nameof(handoverNo), maxLength: 64);
        OutboundReviewOrderId = outboundReviewOrderId != Guid.Empty ? outboundReviewOrderId : throw new BusinessException("复核单Id不能为空。");
        Remark = remark?.Trim();
        Status = OutboundHandoverStatus.Created;
    }

    public static OutboundHandoverOrder Create(Guid id, string handoverNo, Guid outboundReviewOrderId, string? remark = null)
    {
        return new OutboundHandoverOrder(id, handoverNo, outboundReviewOrderId, remark);
    }

    public OutboundHandoverLine AddLine(
        Guid lineId,
        Guid reviewLineId,
        Guid salesShipmentId,
        Guid salesShipmentDetailId,
        Guid salesShipmentAllocationId,
        Guid productId,
        string productCode,
        string productName,
        decimal qty,
        Guid containerId,
        string containerCode,
        Guid locationId,
        string locationCode,
        string batchNo,
        string sn)
    {
        EnsureEditable();

        var line = new OutboundHandoverLine(
            lineId,
            Id,
            reviewLineId,
            salesShipmentId,
            salesShipmentDetailId,
            salesShipmentAllocationId,
            productId,
            productCode,
            productName,
            qty,
            containerId,
            containerCode,
            locationId,
            locationCode,
            batchNo,
            sn);

        _lines.Add(line);
        return line;
    }

    public void Complete()
    {
        if (Status == OutboundHandoverStatus.Completed)
        {
            return;
        }

        if (Status == OutboundHandoverStatus.Cancelled)
        {
            throw new BusinessException("已取消的交接单不允许完成。")
                .WithData("Status", Status);
        }

        if (!_lines.Any())
        {
            throw new BusinessException("交接单至少需要一条明细。");
        }

        Status = OutboundHandoverStatus.Completed;
    }

    public void Cancel()
    {
        if (Status == OutboundHandoverStatus.Completed)
        {
            throw new BusinessException("已完成的交接单不允许取消。")
                .WithData("Status", Status);
        }

        Status = OutboundHandoverStatus.Cancelled;
    }

    private void EnsureEditable()
    {
        if (Status != OutboundHandoverStatus.Created)
        {
            throw new BusinessException("仅新建状态的交接单允许修改。")
                .WithData("Status", Status);
        }
    }
}

public class OutboundHandoverLine : Entity<Guid>
{
    public Guid OutboundHandoverOrderId { get; private set; }
    public Guid ReviewLineId { get; private set; }
    public Guid SalesShipmentId { get; private set; }
    public Guid SalesShipmentDetailId { get; private set; }
    public Guid SalesShipmentAllocationId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductCode { get; private set; }
    public string ProductName { get; private set; }
    public decimal Qty { get; private set; }
    public Guid ContainerId { get; private set; }
    public string ContainerCode { get; private set; }
    public Guid LocationId { get; private set; }
    public string LocationCode { get; private set; }
    public string BatchNo { get; private set; }
    public string SN { get; private set; }

    protected OutboundHandoverLine()
    {
        ProductCode = string.Empty;
        ProductName = string.Empty;
        ContainerCode = string.Empty;
        LocationCode = string.Empty;
        BatchNo = string.Empty;
        SN = string.Empty;
    }

    internal OutboundHandoverLine(
        Guid id,
        Guid outboundHandoverOrderId,
        Guid reviewLineId,
        Guid salesShipmentId,
        Guid salesShipmentDetailId,
        Guid salesShipmentAllocationId,
        Guid productId,
        string productCode,
        string productName,
        decimal qty,
        Guid containerId,
        string containerCode,
        Guid locationId,
        string locationCode,
        string batchNo,
        string sn)
        : base(id)
    {
        OutboundHandoverOrderId = outboundHandoverOrderId != Guid.Empty ? outboundHandoverOrderId : throw new BusinessException("交接单Id不能为空。");
        ReviewLineId = reviewLineId != Guid.Empty ? reviewLineId : throw new BusinessException("复核单行Id不能为空。");
        SalesShipmentId = salesShipmentId != Guid.Empty ? salesShipmentId : throw new BusinessException("销售发货单Id不能为空。");
        SalesShipmentDetailId = salesShipmentDetailId != Guid.Empty ? salesShipmentDetailId : throw new BusinessException("销售发货明细Id不能为空。");
        SalesShipmentAllocationId = salesShipmentAllocationId != Guid.Empty ? salesShipmentAllocationId : throw new BusinessException("销售发货分配Id不能为空。");
        ProductId = productId != Guid.Empty ? productId : throw new BusinessException("物料Id不能为空。");
        ProductCode = Check.NotNullOrWhiteSpace(productCode, nameof(productCode), maxLength: 50);
        ProductName = Check.NotNullOrWhiteSpace(productName, nameof(productName), maxLength: 200);
        Qty = qty > 0 ? qty : throw new BusinessException("交接数量必须大于0。");
        ContainerId = containerId != Guid.Empty ? containerId : throw new BusinessException("容器Id不能为空。");
        ContainerCode = Check.NotNullOrWhiteSpace(containerCode, nameof(containerCode), maxLength: 64);
        LocationId = locationId != Guid.Empty ? locationId : throw new BusinessException("库位Id不能为空。");
        LocationCode = Check.NotNullOrWhiteSpace(locationCode, nameof(locationCode), maxLength: 64);
        BatchNo = Check.NotNullOrWhiteSpace(batchNo, nameof(batchNo), maxLength: 100);
        SN = Check.NotNullOrWhiteSpace(sn, nameof(sn), maxLength: 100);
    }
}


