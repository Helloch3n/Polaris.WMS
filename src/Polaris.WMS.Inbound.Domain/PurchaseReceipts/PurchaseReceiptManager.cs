using Polaris.WMS.BillNumbers;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Polaris.WMS.Inbound.Domain.PurchaseReceipts;

/// <summary>
/// 采购收货单领域服务，负责创建聚合与关键业务编排。
/// </summary>
public class PurchaseReceiptManager(
    IRepository<PurchaseReceipt, Guid> purchaseReceiptRepository,
    IBillNumberGenerator billNumberGenerator)
    : DomainService
{
    /// <summary>
    /// 创建采购收货单聚合（仅构建实体，不执行持久化）。
    /// </summary>
    public async Task<PurchaseReceipt> CreateAsync(
        Guid id,
        string sourceDocType,
        string sourceDocNo,
        Guid? supplierId,
        string? supplierName,
        string? remark,
        IReadOnlyCollection<PurchaseReceiptDetailCreateInfo> details)
    {
        if (details == null || details.Count == 0)
        {
            throw new BusinessException("采购收货单至少需要一条明细。");
        }

        var receiptNo = await billNumberGenerator.GetNextNumberAsync("PR");
        var normalizedReceiptNo = Check.NotNullOrWhiteSpace(receiptNo, nameof(receiptNo));

        var exists = await purchaseReceiptRepository.AnyAsync(x => x.ReceiptNo == normalizedReceiptNo);
        if (exists)
        {
            throw new BusinessException("采购收货单号已存在，请重试。")
                .WithData("单号", normalizedReceiptNo);
        }

        var receipt = new PurchaseReceipt(
            id,
            normalizedReceiptNo,
            sourceDocType,
            sourceDocNo,
            supplierId,
            supplierName,
            remark);

        foreach (var detail in details)
        {
            if (detail.ProductId == Guid.Empty)
            {
                throw new BusinessException("明细中的物料Id不能为空。");
            }

            if (detail.ContainerId == Guid.Empty)
            {
                throw new BusinessException("明细中的容器Id不能为空。");
            }

            if (detail.LocationId == Guid.Empty)
            {
                throw new BusinessException("明细中的库位Id不能为空。");
            }

            if (detail.ReceivedQuantity <= 0)
            {
                throw new BusinessException("明细中的收货数量必须大于 0。");
            }

            receipt.AddDetail(
                detail.Id,
                detail.ProductId,
                detail.ProductCode,
                detail.ProductName,
                detail.ReceivedQuantity,
                detail.ContainerId,
                detail.ContainerCode,
                detail.LocationId,
                detail.LocationCode,
                detail.SourceDetailId,
                detail.BatchNo);
        }

        return receipt;
    }

    /// <summary>
    /// 变更指定明细的 ERP 同步状态。
    /// </summary>
    public async Task ChangeDetailErpSyncStatusAsync(
        Guid purchaseReceiptId,
        Guid detailId,
        PurchaseReceiptErpSyncStatus newStatus,
        string? errorMessage = null)
    {
        var queryable = await purchaseReceiptRepository.WithDetailsAsync(x => x.Details);
        var receipt = await AsyncExecuter.FirstOrDefaultAsync(queryable.Where(x => x.Id == purchaseReceiptId));

        if (receipt == null)
        {
            throw new BusinessException("采购收货单不存在。")
                .WithData("PurchaseReceiptId", purchaseReceiptId);
        }

        var detail = receipt.Details.FirstOrDefault(x => x.Id == detailId);
        if (detail == null)
        {
            throw new BusinessException("采购收货明细不存在。")
                .WithData("DetailId", detailId);
        }

        detail.ChangeErpSyncStatus(newStatus, errorMessage);
        await purchaseReceiptRepository.UpdateAsync(receipt, autoSave: true);
    }
}

/// <summary>
/// 创建采购收货单时的明细输入模型（领域层）。
/// </summary>
public class PurchaseReceiptDetailCreateInfo
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal ReceivedQuantity { get; set; }
    public Guid ContainerId { get; set; }
    public string ContainerCode { get; set; } = string.Empty;
    public Guid LocationId { get; set; }
    public string LocationCode { get; set; } = string.Empty;
    public Guid? SourceDetailId { get; set; }
    public string? BatchNo { get; set; }
}



