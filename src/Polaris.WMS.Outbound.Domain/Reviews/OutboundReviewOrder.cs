using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace Polaris.WMS.Outbound.Domain.Reviews;

/// <summary>
/// 出库复核单聚合根。
/// </summary>
public class OutboundReviewOrder : FullAuditedAggregateRoot<Guid>
{
    public string ReviewNo { get; private set; }

    public Guid PickListId { get; private set; }

    public OutboundReviewStatus Status { get; private set; }

    public string? Remark { get; private set; }

    private readonly List<OutboundReviewLine> _lines = new();
    public IReadOnlyCollection<OutboundReviewLine> Lines => _lines;

    protected OutboundReviewOrder()
    {
        ReviewNo = string.Empty;
        Status = OutboundReviewStatus.Created;
    }

    internal OutboundReviewOrder(Guid id, string reviewNo, Guid pickListId, string? remark = null)
        : base(id)
    {
        ReviewNo = Check.NotNullOrWhiteSpace(reviewNo, nameof(reviewNo), maxLength: 64);
        PickListId = pickListId != Guid.Empty ? pickListId : throw new BusinessException("拣货单Id不能为空。");
        Remark = remark?.Trim();
        Status = OutboundReviewStatus.Created;
    }

    public static OutboundReviewOrder Create(Guid id, string reviewNo, Guid pickListId, string? remark = null)
    {
        return new OutboundReviewOrder(id, reviewNo, pickListId, remark);
    }

    public OutboundReviewLine AddLine(
        Guid lineId,
        Guid pickListLineId,
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

        var line = new OutboundReviewLine(
            lineId,
            Id,
            pickListLineId,
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
        if (Status == OutboundReviewStatus.Completed)
        {
            return;
        }

        if (Status == OutboundReviewStatus.Cancelled)
        {
            throw new BusinessException("已取消的复核单不允许完成。")
                .WithData("Status", Status);
        }

        if (!_lines.Any())
        {
            throw new BusinessException("复核单至少需要一条明细。");
        }

        Status = OutboundReviewStatus.Completed;
    }

    public void Cancel()
    {
        if (Status == OutboundReviewStatus.Completed)
        {
            throw new BusinessException("已完成的复核单不允许取消。")
                .WithData("Status", Status);
        }

        Status = OutboundReviewStatus.Cancelled;
    }

    private void EnsureEditable()
    {
        if (Status != OutboundReviewStatus.Created)
        {
            throw new BusinessException("仅新建状态的复核单允许修改。")
                .WithData("Status", Status);
        }
    }
}

public class OutboundReviewLine : Entity<Guid>
{
    public Guid OutboundReviewOrderId { get; private set; }
    public Guid PickListLineId { get; private set; }
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

    protected OutboundReviewLine()
    {
        ProductCode = string.Empty;
        ProductName = string.Empty;
        ContainerCode = string.Empty;
        LocationCode = string.Empty;
        BatchNo = string.Empty;
        SN = string.Empty;
    }

    internal OutboundReviewLine(
        Guid id,
        Guid outboundReviewOrderId,
        Guid pickListLineId,
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
        OutboundReviewOrderId = outboundReviewOrderId != Guid.Empty ? outboundReviewOrderId : throw new BusinessException("复核单Id不能为空。");
        PickListLineId = pickListLineId != Guid.Empty ? pickListLineId : throw new BusinessException("拣货单行Id不能为空。");
        SalesShipmentId = salesShipmentId != Guid.Empty ? salesShipmentId : throw new BusinessException("销售发货单Id不能为空。");
        SalesShipmentDetailId = salesShipmentDetailId != Guid.Empty ? salesShipmentDetailId : throw new BusinessException("销售发货明细Id不能为空。");
        SalesShipmentAllocationId = salesShipmentAllocationId != Guid.Empty ? salesShipmentAllocationId : throw new BusinessException("销售发货分配Id不能为空。");
        ProductId = productId != Guid.Empty ? productId : throw new BusinessException("物料Id不能为空。");
        ProductCode = Check.NotNullOrWhiteSpace(productCode, nameof(productCode), maxLength: 50);
        ProductName = Check.NotNullOrWhiteSpace(productName, nameof(productName), maxLength: 200);
        Qty = qty > 0 ? qty : throw new BusinessException("复核数量必须大于0。");
        ContainerId = containerId != Guid.Empty ? containerId : throw new BusinessException("容器Id不能为空。");
        ContainerCode = Check.NotNullOrWhiteSpace(containerCode, nameof(containerCode), maxLength: 64);
        LocationId = locationId != Guid.Empty ? locationId : throw new BusinessException("库位Id不能为空。");
        LocationCode = Check.NotNullOrWhiteSpace(locationCode, nameof(locationCode), maxLength: 64);
        BatchNo = Check.NotNullOrWhiteSpace(batchNo, nameof(batchNo), maxLength: 100);
        SN = Check.NotNullOrWhiteSpace(sn, nameof(sn), maxLength: 100);
    }
}


