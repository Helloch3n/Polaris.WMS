using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp;

namespace Polaris.WMS.Inbound.Domain.PurchaseReceipts;

/// <summary>
/// 采购收货单（聚合根）。
/// 记录月台实际发生的一次收货行为，并聚合收货明细。
/// </summary>
public class PurchaseReceipt : FullAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// WMS 内部收货流水号（如：PR-20231001-001）。
    /// </summary>
    public string ReceiptNo { get; private set; }

    /// <summary>
    /// 源单据类型（例如 ASN/PO）。
    /// </summary>
    public string SourceDocType { get; private set; }

    /// <summary>
    /// 源单据号（ASN 单号或 PO 单号）。
    /// </summary>
    public string SourceDocNo { get; private set; }

    /// <summary>
    /// 供应商 Id（可空）。
    /// </summary>
    public Guid? SupplierId { get; private set; }

    /// <summary>
    /// 供应商名称快照（可空）。
    /// </summary>
    public string? SupplierName { get; private set; }

    /// <summary>
    /// 业务备注。
    /// </summary>
    public string? Remark { get; private set; }

    private readonly List<PurchaseReceiptDetail> _details = new();

    /// <summary>
    /// 收货明细（只读集合）。
    /// </summary>
    public IReadOnlyCollection<PurchaseReceiptDetail> Details => _details;

    protected PurchaseReceipt() { }

    internal PurchaseReceipt(
        Guid id,
        string receiptNo,
        string sourceDocType,
        string sourceDocNo,
        Guid? supplierId = null,
        string? supplierName = null,
        string? remark = null) : base(id)
    {
        ReceiptNo = Check.NotNullOrWhiteSpace(receiptNo, nameof(receiptNo), maxLength: 64);
        SourceDocType = Check.NotNullOrWhiteSpace(sourceDocType, nameof(sourceDocType), maxLength: 32);
        SourceDocNo = Check.NotNullOrWhiteSpace(sourceDocNo, nameof(sourceDocNo), maxLength: 64);
        SupplierId = supplierId;
        SupplierName = supplierName?.Trim();
        Remark = remark?.Trim();
    }

    /// <summary>
    /// 领域方法：向收货单中添加收货明细。
    /// </summary>
    public PurchaseReceiptDetail AddDetail(
        Guid id,
        Guid productId,
        string productCode,
        string productName,
        decimal receivedQuantity,
        Guid containerId,
        string containerCode,
        Guid locationId,
        Guid locationCode,
        Guid? sourceDetailId = null,
        string? batchNo = null)
    {
        var detail = new PurchaseReceiptDetail(
            id,
            Id,
            productId,
            productCode,
            productName,
            receivedQuantity,
            containerId,
            containerCode,
            locationId,
            locationCode,
            sourceDetailId,
            batchNo
        );

        _details.Add(detail);
        return detail;
    }

    /// <summary>
    /// 更新备注。
    /// </summary>
    public void UpdateRemark(string? remark)
    {
        Remark = remark?.Trim();
    }
}