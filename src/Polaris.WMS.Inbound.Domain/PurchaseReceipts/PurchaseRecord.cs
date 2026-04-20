using Volo.Abp.Domain.Entities;
using Volo.Abp;

namespace Polaris.WMS.Inbound.Domain.PurchaseReceipts;

public class PurchaseRecord : Entity<Guid>
{
    /// <summary>
    /// 关联的采购收货单（聚合根）Id。
    /// </summary>
    public Guid PurchaseReceiptId { get; private set; }

    /// <summary>
    /// 关联的采购收货单明细 Id。
    /// </summary>
    public Guid PurchaseReceiptDetailId { get; private set; }

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
    public string ProductName { get; private set; } = string.Empty;

    /// <summary>
    /// 物料编码快照。
    /// </summary>
    public string ProductCode { get; private set; } = string.Empty;

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
    public string ContainerCode { get; private set; } = string.Empty;

    /// <summary>
    /// 物理位置 Id：月台或收货暂存区的 Location Id（仅保存 Id）。
    /// </summary>
    public Guid LocationId { get; private set; }

    /// <summary>
    /// 位置编码快照。
    /// </summary>
    public string LocationCode { get; private set; } = string.Empty;

    /// <summary>
    /// 批次号（如果有）。
    /// </summary>
    public string? BatchNo { get; private set; }

    public string? SupplierBatchNo { get; private set; }

    protected PurchaseRecord()
    {
    }

    internal PurchaseRecord(
        Guid id,
        Guid purchaseReceiptId,
        Guid purchaseReceiptDetailId,
        Guid? sourceDetailId,
        Guid productId,
        string productName,
        string productCode,
        decimal receivedQuantity,
        Guid containerId,
        string containerCode,
        Guid locationId,
        string locationCode,
        string? batchNo,
        string? supplierBatchNo) : base(id)
    {
        if (purchaseReceiptId == Guid.Empty)
        {
            throw new BusinessException("采购收货单Id不能为空。");
        }

        if (purchaseReceiptDetailId == Guid.Empty)
        {
            throw new BusinessException("采购收货明细Id不能为空。");
        }

        if (productId == Guid.Empty)
        {
            throw new BusinessException("物料Id不能为空。");
        }

        if (receivedQuantity <= 0)
        {
            throw new BusinessException("实收数量必须大于 0。");
        }

        if (containerId == Guid.Empty)
        {
            throw new BusinessException("容器Id不能为空。");
        }

        if (locationId == Guid.Empty)
        {
            throw new BusinessException("库位Id不能为空。");
        }

        PurchaseReceiptId = purchaseReceiptId;
        PurchaseReceiptDetailId = purchaseReceiptDetailId;
        SourceDetailId = sourceDetailId;
        ProductId = productId;
        ProductName = Check.NotNullOrWhiteSpace(productName, nameof(productName), maxLength: 256);
        ProductCode = Check.NotNullOrWhiteSpace(productCode, nameof(productCode), maxLength: 64);
        ReceivedQuantity = receivedQuantity;
        ContainerId = containerId;
        ContainerCode = Check.NotNullOrWhiteSpace(containerCode, nameof(containerCode), maxLength: 64);
        LocationId = locationId;
        LocationCode = Check.NotNullOrWhiteSpace(locationCode, nameof(locationCode), maxLength: 64);
        BatchNo = batchNo?.Trim();
        SupplierBatchNo = supplierBatchNo?.Trim();
    }
}