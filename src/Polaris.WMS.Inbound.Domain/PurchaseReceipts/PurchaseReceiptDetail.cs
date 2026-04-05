using Volo.Abp.Domain.Entities;
using Volo.Abp;

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

    /// <summary>
    /// 来源追溯：可选的源明细 Id（例如 ASN 明细或采购订单明细的 Id）。
    /// </summary>
    public Guid? SourceDetailId { get; private set; }

    /// <summary>
    /// 物料 Id，仅保存 Id 以遵循限界上下文边界规则。
    /// </summary>
    public Guid ProductId { get; private set; }

    /// <summary>
    /// 物料名称快照。
    /// </summary>
    public string ProductName { get; private set; }

    /// <summary>
    /// 物料编码快照。
    /// </summary>
    public string ProductCode { get; private set; }

    /// <summary>
    /// 实际接收的数量（物理托盘上的数量）。
    /// 注意：库存相关的可用量/已分配量不应在此实体中计算或存储。
    /// </summary>
    public decimal ReceivedQuantity { get; private set; }

    /// <summary>
    /// 容器 Id（托盘/箱 Id）。
    /// </summary>
    public Guid ContainerId { get; private set; }

    /// <summary>
    /// 容器编码快照。
    /// </summary>
    public string ContainerCode { get; private set; }

    /// <summary>
    /// 物理位置 Id：月台或收货暂存区的 Location Id（仅保存 Id）。
    /// </summary>
    public Guid LocationId { get; private set; }

    /// <summary>
    /// 位置编码快照。
    /// </summary>
    public string LocationCode { get; private set; }

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

    protected PurchaseReceiptDetail() { }

    internal PurchaseReceiptDetail(
        Guid id,
        Guid purchaseReceiptId,
        Guid productId,
        string productCode,
        string productName,
        decimal receivedQuantity,
        Guid containerId,
        string containerCode,
        Guid locationId,
        string locationCode,
        Guid? sourceDetailId = null,
        string? batchNo = null) : base(id)
    {
        PurchaseReceiptId = purchaseReceiptId;
        ProductId = productId;
        ProductCode = Check.NotNullOrWhiteSpace(productCode, nameof(productCode), maxLength: 64);
        ProductName = Check.NotNullOrWhiteSpace(productName, nameof(productName), maxLength: 256);
        ReceivedQuantity = receivedQuantity;
        ContainerId = containerId;
        ContainerCode = Check.NotNullOrWhiteSpace(containerCode, nameof(containerCode), maxLength: 64);
        LocationId = locationId;
        LocationCode = Check.NotNullOrWhiteSpace(locationCode, nameof(locationCode), maxLength: 64);
        SourceDetailId = sourceDetailId;
        BatchNo = batchNo?.Trim();

        // 初始状态为未同步
        ErpSyncStatus = PurchaseReceiptErpSyncStatus.NotSynced;
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