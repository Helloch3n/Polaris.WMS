using Polaris.WMS.BillNumbers;
using Polaris.WMS.Inbound.Events;
using Polaris.WMS.Inbound.Domain.Integration.Inventories;
using Polaris.WMS.Inbound.Domain.Integration.Containers;
using Polaris.WMS.Inventories.Invnentory;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Local;

namespace Polaris.WMS.Inbound.Domain.PurchaseReceipts;

/// <summary>
/// 采购收货单领域服务，负责创建聚合与关键业务编排。
/// </summary>
public class PurchaseReceiptManager(
    IRepository<PurchaseReceipt, Guid> purchaseReceiptRepository,
    IRepository<PurchaseRecord, Guid> purchaseRecordRepository,
    ILocalEventBus localEventBus,
    IBillNumberGenerator billNumberGenerator)
    : DomainService
{
    private IExternalContainerProvider ExternalContainerProvider =>
        LazyServiceProvider.LazyGetRequiredService<IExternalContainerProvider>();

    private IExternalInventoryProvider ExternalInventoryProvider =>
        LazyServiceProvider.LazyGetRequiredService<IExternalInventoryProvider>();

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

            if (detail.ExpectedQuantity <= 0)
            {
                throw new BusinessException("明细中的应收数量必须大于 0。");
            }

            receipt.AddDetail(
                detail.Id,
                detail.ProductId,
                detail.ProductCode,
                detail.ProductName,
                detail.ExpectedQuantity,
                detail.SourceAsnLineId,
                detail.SourcePoLineId,
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

    /// <summary>
    /// 向采购收货明细新增收货记录。
    /// </summary>
    public async Task AddRecordsAsync(
        Guid purchaseReceiptId,
        Guid detailId,
        IReadOnlyCollection<PurchaseRecordCreateInfo> records)
    {
        if (records == null || records.Count == 0)
        {
            throw new BusinessException("至少需要一条收货记录。");
        }

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

        foreach (var record in records)
        {
            detail.AddRecord(
                record.Id,
                record.ReceivedQuantity,
                record.ContainerId,
                record.ContainerCode,
                record.LocationId,
                record.LocationCode,
                record.BatchNo,
                record.SupplierBatchNo);
        }

        await purchaseReceiptRepository.UpdateAsync(receipt, autoSave: true);
    }

    /// <summary>
    /// 审核并执行指定明细。
    /// </summary>
    public async Task ApproveAndExecuteDetailAsync(Guid purchaseReceiptId, Guid detailId)
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

        if (detail.ReceivedQuantity <= 0)
        {
            throw new BusinessException("采购收货明细未收货，无法审核执行。")
                .WithData("DetailId", detailId);
        }

        if (detail.ErpSyncStatus == PurchaseReceiptErpSyncStatus.Received ||
            detail.ErpSyncStatus == PurchaseReceiptErpSyncStatus.Delivered)
        {
            throw new BusinessException("采购收货明细已审核执行，请勿重复操作。")
                .WithData("DetailId", detailId);
        }

        /*if (detail.ReceivedQuantity != detail.ExpectedQuantity)
        {
            throw new BusinessException("采购收货明细实收数量与应收数量不一致，无法审核执行。")
                .WithData("DetailId", detailId)
                .WithData("ExpectedQuantity", detail.ExpectedQuantity)
                .WithData("ReceivedQuantity", detail.ReceivedQuantity);
        }*/

        var records = await purchaseRecordRepository.GetListAsync(x => x.PurchaseReceiptDetailId == detailId);
        if (records.Count == 0)
        {
            throw new BusinessException("采购收货明细没有收货记录，无法审核执行。")
                .WithData("DetailId", detailId);
        }

        var recordQtyTotal = records.Sum(x => x.ReceivedQuantity);
        /*if (recordQtyTotal != detail.ExpectedQuantity)
        {
            throw new BusinessException("收货记录累计数量与应收数量不一致，无法审核执行。")
                .WithData("DetailId", detailId)
                .WithData("ExpectedQuantity", detail.ExpectedQuantity)
                .WithData("RecordsReceivedQuantity", recordQtyTotal);
        }*/

        foreach (var record in records)
        {
            var container = await ExternalContainerProvider.GetContainerAsync(record.ContainerId);
            if (container == null)
            {
                throw new BusinessException("系统中不存在对应的容器。")
                    .WithData("ContainerId", record.ContainerId)
                    .WithData("ContainerCode", record.ContainerCode);
            }

            if (container.CurrentLocationId != record.LocationId)
            {
                // 隐式调拨：容器当前库位与收货记录目标库位不一致时，先执行移库。
                await ExternalContainerProvider.MoveContainerAsync(record.ContainerId, record.LocationId, receipt.ReceiptNo);
            }

            var receiveInfo = new ExternalProductionReceiveInfo
            {
                OrderNo = receipt.ReceiptNo,
                ReelId = record.ContainerId,
                ActualLocationId = record.LocationId,
                Items =
                [
                    new ExternalProductionReceiveInfoItem
                    {
                        ProductId = record.ProductId,
                        Qty = record.ReceivedQuantity,
                        Weight = 0,
                        BatchNo = record.BatchNo ?? detail.BatchNo ?? string.Empty,
                        RelatedOrderNo = receipt.SourceDocNo,
                        RelatedOrderNoLineNo = record.SourceDetailId?.ToString() ?? detail.Id.ToString("N"),
                        SN = $"PR-{record.Id:N}",
                        Unit = "PCS",
                        CraftVersion = string.Empty,
                        LayerIndex = 0,
                        Status = InventoryStatus.Hold
                    }
                ]
            };

            await ExternalInventoryProvider.ReceiveProductionAsync(receiveInfo);
        }

        detail.ChangeErpSyncStatus(PurchaseReceiptErpSyncStatus.Received);
        await purchaseReceiptRepository.UpdateAsync(receipt, autoSave: true);

        await localEventBus.PublishAsync(new PurchaseReceiptExecutedLocalEto
        {
            SourceAsnLineId = detail.SourceAsnLineId,
            SourcePoLineId = detail.SourcePoLineId,
            ReceiptOrderId = receipt.Id,
            ProductId = detail.ProductId,
            Quantity = recordQtyTotal,
            ReceiptOrderLineId = detail.Id
        });
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
    public decimal ExpectedQuantity { get; set; }
    public Guid? SourceAsnLineId { get; set; }
    public Guid? SourcePoLineId { get; set; }
    public string? BatchNo { get; set; }
}

/// <summary>
/// 创建采购收货记录时的输入模型（领域层）。
/// </summary>
public class PurchaseRecordCreateInfo
{
    public Guid Id { get; set; }
    public decimal ReceivedQuantity { get; set; }
    public Guid ContainerId { get; set; }
    public string ContainerCode { get; set; } = string.Empty;
    public Guid LocationId { get; set; }
    public string LocationCode { get; set; } = string.Empty;
    public string? BatchNo { get; set; }
    public string? SupplierBatchNo { get; set; }
}



