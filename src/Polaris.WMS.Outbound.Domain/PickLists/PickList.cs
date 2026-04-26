using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace Polaris.WMS.Outbound.Domain.PickLists;

/// <summary>
/// 出库拣货单聚合根。
/// </summary>
public class PickList : FullAuditedAggregateRoot<Guid>
{
    public string PickNo { get; private set; }

    public Guid WaveOrderId { get; private set; }

    public Guid TargetLocationId { get; private set; }

    public string TargetLocationCode { get; private set; }

    public PickListStatus Status { get; private set; }

    public string? Remark { get; private set; }

    private readonly List<PickListLine> _lines = new();
    public IReadOnlyCollection<PickListLine> Lines => _lines;

    protected PickList()
    {
        PickNo = string.Empty;
        TargetLocationCode = string.Empty;
        Status = PickListStatus.Created;
    }

    internal PickList(Guid id, string pickNo, Guid waveOrderId, Guid targetLocationId, string targetLocationCode, string? remark = null)
        : base(id)
    {
        PickNo = Check.NotNullOrWhiteSpace(pickNo, nameof(pickNo), maxLength: 64);
        WaveOrderId = waveOrderId != Guid.Empty ? waveOrderId : throw new BusinessException("波次单Id不能为空。");
        TargetLocationId = targetLocationId != Guid.Empty ? targetLocationId : throw new BusinessException("拣货目标库位Id不能为空。");
        TargetLocationCode = Check.NotNullOrWhiteSpace(targetLocationCode, nameof(targetLocationCode), maxLength: 64);
        Remark = remark?.Trim();
        Status = PickListStatus.Created;
    }

    public static PickList Create(Guid id, string pickNo, Guid waveOrderId, Guid targetLocationId, string targetLocationCode, string? remark = null)
    {
        return new PickList(id, pickNo, waveOrderId, targetLocationId, targetLocationCode, remark);
    }

    public PickListLine AddLine(
        Guid lineId,
        Guid salesShipmentId,
        string salesShipmentNo,
        Guid salesShipmentDetailId,
        Guid salesShipmentAllocationId,
        Guid productId,
        string productCode,
        string productName,
        decimal qty,
        Guid containerId,
        string containerCode,
        Guid sourceLocationId,
        string sourceLocationCode,
        string batchNo,
        string sn)
    {
        EnsureEditable();

        var line = new PickListLine(
            lineId,
            Id,
            salesShipmentId,
            salesShipmentNo,
            salesShipmentDetailId,
            salesShipmentAllocationId,
            productId,
            productCode,
            productName,
            qty,
            containerId,
            containerCode,
            sourceLocationId,
            sourceLocationCode,
            TargetLocationId,
            TargetLocationCode,
            batchNo,
            sn);

        _lines.Add(line);
        return line;
    }

    public void AttachTask(Guid lineId, Guid moveTaskId, string moveTaskNo)
    {
        var line = GetLine(lineId);
        line.AttachTask(moveTaskId, moveTaskNo);
        RefreshStatus();
    }

    public void MarkLinePicked(Guid moveTaskId, Guid actualLocationId)
    {
        var line = _lines.FirstOrDefault(x => x.MoveTaskId == moveTaskId);
        if (line == null)
        {
            throw new BusinessException("拣货任务未匹配到拣货单行。")
                .WithData("MoveTaskId", moveTaskId);
        }

        line.MarkPicked(actualLocationId);
        RefreshStatus();
    }

    public void RefreshStatus()
    {
        if (!_lines.Any())
        {
            Status = PickListStatus.Created;
            return;
        }

        if (_lines.All(x => x.IsPicked))
        {
            Status = PickListStatus.Picked;
            return;
        }

        if (_lines.Any(x => x.IsPicked))
        {
            Status = PickListStatus.Picking;
            return;
        }

        if (_lines.Any(x => x.MoveTaskId.HasValue))
        {
            Status = PickListStatus.TaskCreated;
            return;
        }

        Status = PickListStatus.Created;
    }

    public void Cancel()
    {
        if (Status == PickListStatus.Picked)
        {
            throw new BusinessException("已完成拣货的拣货单不允许取消。")
                .WithData("Status", Status);
        }

        Status = PickListStatus.Cancelled;
    }

    private PickListLine GetLine(Guid lineId)
    {
        var line = _lines.FirstOrDefault(x => x.Id == lineId);
        if (line == null)
        {
            throw new BusinessException("拣货单行不存在。")
                .WithData("LineId", lineId);
        }

        return line;
    }

    private void EnsureEditable()
    {
        if (Status != PickListStatus.Created)
        {
            throw new BusinessException("仅新建状态的拣货单允许修改。")
                .WithData("Status", Status);
        }
    }
}

public class PickListLine : Entity<Guid>
{
    public Guid PickListId { get; private set; }

    public Guid SalesShipmentId { get; private set; }

    public string SalesShipmentNo { get; private set; }

    public Guid SalesShipmentDetailId { get; private set; }

    public Guid SalesShipmentAllocationId { get; private set; }

    public Guid ProductId { get; private set; }

    public string ProductCode { get; private set; }

    public string ProductName { get; private set; }

    public decimal Qty { get; private set; }

    public Guid ContainerId { get; private set; }

    public string ContainerCode { get; private set; }

    public Guid SourceLocationId { get; private set; }

    public string SourceLocationCode { get; private set; }

    public Guid TargetLocationId { get; private set; }

    public string TargetLocationCode { get; private set; }

    public Guid? MoveTaskId { get; private set; }

    public string? MoveTaskNo { get; private set; }

    public Guid? ActualLocationId { get; private set; }

    public string BatchNo { get; private set; }

    public string SN { get; private set; }

    public bool IsPicked { get; private set; }

    protected PickListLine()
    {
        SalesShipmentNo = string.Empty;
        ProductCode = string.Empty;
        ProductName = string.Empty;
        ContainerCode = string.Empty;
        SourceLocationCode = string.Empty;
        TargetLocationCode = string.Empty;
        BatchNo = string.Empty;
        SN = string.Empty;
    }

    internal PickListLine(
        Guid id,
        Guid pickListId,
        Guid salesShipmentId,
        string salesShipmentNo,
        Guid salesShipmentDetailId,
        Guid salesShipmentAllocationId,
        Guid productId,
        string productCode,
        string productName,
        decimal qty,
        Guid containerId,
        string containerCode,
        Guid sourceLocationId,
        string sourceLocationCode,
        Guid targetLocationId,
        string targetLocationCode,
        string batchNo,
        string sn)
        : base(id)
    {
        PickListId = pickListId != Guid.Empty ? pickListId : throw new BusinessException("拣货单Id不能为空。");
        SalesShipmentId = salesShipmentId != Guid.Empty ? salesShipmentId : throw new BusinessException("销售发货单Id不能为空。");
        SalesShipmentNo = Check.NotNullOrWhiteSpace(salesShipmentNo, nameof(salesShipmentNo), maxLength: 64);
        SalesShipmentDetailId = salesShipmentDetailId != Guid.Empty ? salesShipmentDetailId : throw new BusinessException("销售发货明细Id不能为空。");
        SalesShipmentAllocationId = salesShipmentAllocationId != Guid.Empty ? salesShipmentAllocationId : throw new BusinessException("销售发货分配Id不能为空。");
        ProductId = productId != Guid.Empty ? productId : throw new BusinessException("物料Id不能为空。");
        ProductCode = Check.NotNullOrWhiteSpace(productCode, nameof(productCode), maxLength: 50);
        ProductName = Check.NotNullOrWhiteSpace(productName, nameof(productName), maxLength: 200);
        Qty = qty > 0 ? qty : throw new BusinessException("拣货数量必须大于0。");
        ContainerId = containerId != Guid.Empty ? containerId : throw new BusinessException("容器Id不能为空。");
        ContainerCode = Check.NotNullOrWhiteSpace(containerCode, nameof(containerCode), maxLength: 64);
        SourceLocationId = sourceLocationId != Guid.Empty ? sourceLocationId : throw new BusinessException("来源库位Id不能为空。");
        SourceLocationCode = Check.NotNullOrWhiteSpace(sourceLocationCode, nameof(sourceLocationCode), maxLength: 64);
        TargetLocationId = targetLocationId != Guid.Empty ? targetLocationId : throw new BusinessException("目标库位Id不能为空。");
        TargetLocationCode = Check.NotNullOrWhiteSpace(targetLocationCode, nameof(targetLocationCode), maxLength: 64);
        BatchNo = Check.NotNullOrWhiteSpace(batchNo, nameof(batchNo), maxLength: 100);
        SN = Check.NotNullOrWhiteSpace(sn, nameof(sn), maxLength: 100);
        IsPicked = false;
    }

    public void AttachTask(Guid moveTaskId, string moveTaskNo)
    {
        if (MoveTaskId.HasValue)
        {
            throw new BusinessException("拣货单行已绑定任务，不允许重复创建。")
                .WithData("MoveTaskId", MoveTaskId);
        }

        MoveTaskId = moveTaskId != Guid.Empty ? moveTaskId : throw new BusinessException("拣货任务Id不能为空。");
        MoveTaskNo = Check.NotNullOrWhiteSpace(moveTaskNo, nameof(moveTaskNo), maxLength: 64);
    }

    public void MarkPicked(Guid actualLocationId)
    {
        if (!MoveTaskId.HasValue)
        {
            throw new BusinessException("拣货单行尚未绑定任务。")
                .WithData("PickListLineId", Id);
        }

        if (IsPicked)
        {
            return;
        }

        ActualLocationId = actualLocationId != Guid.Empty ? actualLocationId : throw new BusinessException("实际库位Id不能为空。");
        IsPicked = true;
    }
}

