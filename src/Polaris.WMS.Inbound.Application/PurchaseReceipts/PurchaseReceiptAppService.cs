using Microsoft.AspNetCore.Authorization;
using Polaris.WMS.Inbound.Application.Contracts.PurchaseReceipts;
using Polaris.WMS.Inbound.Application.Contracts.PurchaseReceipts.Dtos;
using Polaris.WMS.Inbound.Domain.PurchaseReceipts;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Polaris.WMS.Inbound.Application.PurchaseReceipts;

/// <summary>
/// 采购收货单应用服务。
/// </summary>
[Authorize]
public class PurchaseReceiptAppService(
    PurchaseReceiptManager purchaseReceiptManager,
    IRepository<PurchaseReceipt, Guid> purchaseReceiptRepository)
    : ApplicationService, IPurchaseReceiptAppService
{
    /// <summary>
    /// 创建采购收货单。
    /// </summary>
    public async Task<PurchaseReceiptDto> CreateAsync(CreatePurchaseReceiptDto input)
    {
        var detailInfos = input.Details
            .Select(x => new PurchaseReceiptDetailCreateInfo
            {
                Id = GuidGenerator.Create(),
                ProductId = x.ProductId,
                ProductCode = x.ProductCode,
                ProductName = x.ProductName,
                ReceivedQuantity = x.ReceivedQuantity,
                ContainerId = x.ContainerId,
                ContainerCode = x.ContainerCode,
                LocationId = x.LocationId,
                LocationCode = x.LocationCode,
                SourceDetailId = x.SourceDetailId,
                BatchNo = x.BatchNo
            })
            .ToList();

        // 第一步：由领域服务创建聚合，统一执行单号生成与核心校验
        var receipt = await purchaseReceiptManager.CreateAsync(
            GuidGenerator.Create(),
            input.SourceDocType,
            input.SourceDocNo,
            input.SupplierId,
            input.SupplierName,
            input.Remark,
            detailInfos);

        // 第二步：持久化
        await purchaseReceiptRepository.InsertAsync(receipt, autoSave: true);

        // 第三步：返回详情
        var queryable = await purchaseReceiptRepository.WithDetailsAsync(x => x.Details);
        var persisted = await AsyncExecuter.FirstOrDefaultAsync(queryable.Where(x => x.Id == receipt.Id));
        return ObjectMapper.Map<PurchaseReceipt, PurchaseReceiptDto>(persisted!);
    }

    /// <summary>
    /// 获取采购收货单详情。
    /// </summary>
    public async Task<PurchaseReceiptDto> GetAsync(Guid id)
    {
        var queryable = await purchaseReceiptRepository.WithDetailsAsync(x => x.Details);
        var receipt = await AsyncExecuter.FirstOrDefaultAsync(queryable.Where(x => x.Id == id));

        if (receipt == null)
        {
            throw new EntityNotFoundException(typeof(PurchaseReceipt), id);
        }

        return ObjectMapper.Map<PurchaseReceipt, PurchaseReceiptDto>(receipt);
    }

    /// <summary>
    /// 分页查询采购收货单。
    /// </summary>
    public async Task<PagedResultDto<PurchaseReceiptDto>> GetListAsync(PurchaseReceiptSearchDto input)
    {
        var receiptNo = input.ReceiptNo?.Trim();
        var sourceDocNo = input.SourceDocNo?.Trim();
        var sourceDocType = input.SourceDocType?.Trim();

        var query = (await purchaseReceiptRepository.GetQueryableAsync())
            .WhereIf(!string.IsNullOrWhiteSpace(receiptNo), x => x.ReceiptNo == receiptNo)
            .WhereIf(!string.IsNullOrWhiteSpace(sourceDocNo), x => x.SourceDocNo == sourceDocNo)
            .WhereIf(!string.IsNullOrWhiteSpace(sourceDocType), x => x.SourceDocType == sourceDocType);

        var totalCount = await AsyncExecuter.CountAsync(query);

        var sortingField = input.Sorting?.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        var isDesc = input.Sorting?.Contains("DESC", StringComparison.OrdinalIgnoreCase) == true;

        query = sortingField switch
        {
            nameof(PurchaseReceipt.CreationTime) => isDesc
                ? query.OrderByDescending(x => x.CreationTime)
                : query.OrderBy(x => x.CreationTime),
            nameof(PurchaseReceipt.SourceDocNo) => isDesc
                ? query.OrderByDescending(x => x.SourceDocNo)
                : query.OrderBy(x => x.SourceDocNo),
            _ => query.OrderByDescending(x => x.CreationTime)
        };

        var skipCount = input.SkipCount < 0 ? 0 : input.SkipCount;
        var maxResultCount = input.MaxResultCount <= 0 ? 10 : input.MaxResultCount;
        var items = await AsyncExecuter.ToListAsync(query.Skip(skipCount).Take(maxResultCount));

        return new PagedResultDto<PurchaseReceiptDto>(
            totalCount,
            items.Select(x => ObjectMapper.Map<PurchaseReceipt, PurchaseReceiptDto>(x)).ToList());
    }

    /// <summary>
    /// 更新采购收货明细 ERP 同步状态。
    /// </summary>
    public async Task ChangeDetailErpSyncStatusAsync(ChangePurchaseReceiptDetailErpSyncStatusDto input)
    {
        if (input.PurchaseReceiptId == Guid.Empty)
        {
            throw new BusinessException("采购收货单Id不能为空。");
        }

        if (input.DetailId == Guid.Empty)
        {
            throw new BusinessException("采购收货明细Id不能为空。");
        }

        await purchaseReceiptManager.ChangeDetailErpSyncStatusAsync(
            input.PurchaseReceiptId,
            input.DetailId,
            input.NewStatus,
            input.ErrorMessage);
    }
}



