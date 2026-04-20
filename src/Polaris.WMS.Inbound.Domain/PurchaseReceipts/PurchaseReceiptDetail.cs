using Volo.Abp.Domain.Entities;
using Volo.Abp;
using System.ComponentModel.DataAnnotations;

namespace Polaris.WMS.Inbound.Domain.PurchaseReceipts;

/// <summary>
/// 采购收货单明细。每一行代表月台实际扫码产生的一个物理托盘/容器。
/// 设计说明：
/// - 该实体为入库流程中的明细聚合（非库存聚合）。仅保存与暂收相关的业务信息和跟踪字段。
/// - 跨限界上下文引用必须使用 Id（例如 SkuId、LocationId），禁止导航属性引用其他上下文的实体。
/// </summary>
public class PurchaseReceiptDetail : Entity<Guid>
{
    /// <summary>
    /// 关联的采购收货单（聚合根）Id。
    /// </summary>
    public Guid PurchaseReceiptId { get; private set; }

    public Guid? SourceAsnLineId { get; private set; }
    
    public Guid? SourcePoLineId { get; private set; }

    /// <summary>
    /// 物料 Id，仅保存 Id 以遵循限界上下文边界规则。
    /// </summary>
    public Guid ProductId { get; private set; }

    /// <summary>
    /// 物料名称快照。
    /// </summary>
    public string ProductName { get; private set; } = string.Empty;

    /// <summary>
    /// 物料编码快照。
    /// </summary>
    public string ProductCode { get; private set; } = string.Empty;

    public decimal ExpectedQuantity { get; private set; }

    /// <summary>
    /// 实际接收的数量（物理托盘上的数量）。
    /// 注意：库存相关的可用量/已分配量不应在此实体中计算或存储。
    /// </summary>
    [ConcurrencyCheck]
    public decimal ReceivedQuantity { get; private set; }

    /// <summary>
    /// 批次号（如果有）。
    /// </summary>
    public string? BatchNo { get; private set; }

    /// <summary>
    /// 挂载在该流水明细上的 ERP 同步状态，用于记录暂收到 ERP 的同步进度与错误状态。
    /// </summary>
    public PurchaseReceiptErpSyncStatus ErpSyncStatus { get; private set; }

    /// <summary>
    /// ERP 同步失败时记录的错误信息（成功时为空）。
    /// </summary>
    public string? ErpSyncErrorMessage { get; private set; }

    private readonly List<PurchaseRecord> _records = new();

    /// <summary>
    /// 收货记录（孙实体）。
    /// </summary>
    public IReadOnlyCollection<PurchaseRecord> Records => _records;

    protected PurchaseReceiptDetail() { }

    internal PurchaseReceiptDetail(
        Guid id,
        Guid purchaseReceiptId,
        Guid productId,
        string productCode,
        string productName,
        decimal expectedQuantity,
        Guid? sourceAsnLineId = null,
        Guid? sourcePoLineId = null,
        string? batchNo = null) : base(id)
    {
        PurchaseReceiptId = purchaseReceiptId;
        ProductId = productId;
        ProductCode = Check.NotNullOrWhiteSpace(productCode, nameof(productCode), maxLength: 64);
        ProductName = Check.NotNullOrWhiteSpace(productName, nameof(productName), maxLength: 256);
        ExpectedQuantity = expectedQuantity;
        ReceivedQuantity = 0;
        SourceAsnLineId = sourceAsnLineId;
        SourcePoLineId = sourcePoLineId;
        BatchNo = batchNo?.Trim();

        // 初始状态为未同步
        ErpSyncStatus = PurchaseReceiptErpSyncStatus.NotSynced;
    }

    /// <summary>
    /// 新增收货记录，并同步累计实收数量。
    /// </summary>
    public PurchaseRecord AddRecord(
        Guid id,
        decimal receivedQuantity,
        Guid containerId,
        string containerCode,
        Guid locationId,
        string locationCode,
        string? batchNo = null,
        string? supplierBatchNo = null)
    {
        if (receivedQuantity <= 0)
        {
            throw new BusinessException("收货记录数量必须大于 0。");
        }

        if (containerId == Guid.Empty)
        {
            throw new BusinessException("收货记录的容器Id不能为空。");
        }

        if (locationId == Guid.Empty)
        {
            throw new BusinessException("收货记录的库位Id不能为空。");
        }

        var nextQuantity = ReceivedQuantity + receivedQuantity;
        if (nextQuantity > ExpectedQuantity)
        {
            throw new BusinessException("累计实收数量不能大于应收数量。");
        }

        var record = new PurchaseRecord(
            id,
            PurchaseReceiptId,
            Id,
            SourcePoLineId,
            ProductId,
            ProductName,
            ProductCode,
            receivedQuantity,
            containerId,
            containerCode,
            locationId,
            locationCode,
            batchNo,
            supplierBatchNo);

        _records.Add(record);
        ReceivedQuantity = nextQuantity;
        return record;
    }

    /// <summary>
    /// 更改 ERP 同步状态。
    /// 使用场景：由后台集成 Job、事件处理器或人工操作更新同步结果。
    /// 注意：业务约束（例如仅允许从失败状态重试到对应的成功状态）应由调用方或领域服务校验；
    /// 本方法仅执行简单的状态赋值并记录变更。
    /// </summary>
    /// <param name="newStatus">目标同步状态。</param>
    /// <param name="errorMessage">失败时的错误信息。</param>
    public void ChangeErpSyncStatus(PurchaseReceiptErpSyncStatus newStatus, string? errorMessage = null)
    {
        ErpSyncStatus = newStatus;
        if (newStatus == PurchaseReceiptErpSyncStatus.ReceiveFailed || newStatus == PurchaseReceiptErpSyncStatus.DeliverFailed)
        {
            // 如果是失败状态，强制记录错误信息（截断保证不超出数据库字段长度）
            ErpSyncErrorMessage = errorMessage?.Length > 1000 ? errorMessage.Substring(0, 1000) : errorMessage;
        }
        else
        {
            // 如果成功了，清空错误信息
            ErpSyncErrorMessage = null;
        }
    }

}