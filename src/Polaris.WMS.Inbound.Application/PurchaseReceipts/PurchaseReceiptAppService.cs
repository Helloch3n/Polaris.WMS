using Microsoft.AspNetCore.Authorization;
using Polaris.WMS.Inbound.Application.Contracts.PurchaseReceipts;
using Polaris.WMS.Inbound.Application.Contracts.PurchaseReceipts.Dtos;
using Polaris.WMS.Inbound.Domain.Asns;
using Polaris.WMS.Inbound.Domain.PurchaseReceipts;
using Polaris.WMS.InventoryManage.Application.Contracts.Integration.Containers;
using Polaris.WMS.MasterData.Application.Contracts.Integration.Locations;
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
    IRepository<PurchaseReceipt, Guid> purchaseReceiptRepository,
    IRepository<PurchaseRecord, Guid> purchaseRecordRepository,
    IRepository<AsnDetail, Guid> asnDetailRepository,
    IRepository<Polaris.WMS.Inbound.Domain.PurchaseOrders.PurchaseOrderDetail, Guid> purchaseOrderDetailRepository,
    IContainerIntegrationService containerIntegrationService,
    ILocationIntegrationService locationIntegrationService)
    : ApplicationService, IPurchaseReceiptAppService
{
    /// <summary>
    /// 审核并执行采购收货单。
    /// </summary>
    public async Task ApproveAndExecuteAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new BusinessException("采购收货单Id不能为空。");
        }

        // 1. 加载收货单及明细
        var queryable = await purchaseReceiptRepository.WithDetailsAsync(x => x.Details);
        var receipt = await AsyncExecuter.FirstOrDefaultAsync(queryable.Where(x => x.Id == id));

        if (receipt == null)
        {
            throw new EntityNotFoundException(typeof(PurchaseReceipt), id);
        }

        if (!receipt.Details.Any())
        {
            throw new BusinessException("采购收货单无明细，无法审核执行。");
        }

        // 2. 按明细逐行调用领域服务完成审核执行
        foreach (var detail in receipt.Details)
        {
            await purchaseReceiptManager.ApproveAndExecuteDetailAsync(receipt.Id, detail.Id);
        }
    }

    /// <summary>
    /// 创建采购收货单。
    /// </summary>
    public async Task<PurchaseReceiptDto> CreateAsync(CreatePurchaseReceiptDto input)
    {
        var detailInfos = new List<PurchaseReceiptDetailCreateInfo>(input.Details.Count);
        foreach (var x in input.Details)
        {
            var sourcePoLineId = x.SourcePoLineId;
            if (!sourcePoLineId.HasValue && x.SourceAsnLineId.HasValue)
            {
                var asnDetail = await asnDetailRepository.FirstOrDefaultAsync(d => d.Id == x.SourceAsnLineId.Value);
                if (asnDetail == null)
                {
                    throw new BusinessException("来源ASN明细不存在，无法回填采购单明细。")
                        .WithData("SourceAsnLineId", x.SourceAsnLineId.Value);
                }

                var poDetail = await purchaseOrderDetailRepository.FirstOrDefaultAsync(d =>
                    d.PurchaseOrderId == asnDetail.SourcePoId && d.LineNo == asnDetail.SourcePoLineNo);

                if (poDetail == null)
                {
                    throw new BusinessException("无法根据来源ASN明细匹配采购单明细。")
                        .WithData("SourceAsnLineId", x.SourceAsnLineId.Value)
                        .WithData("SourcePoId", asnDetail.SourcePoId)
                        .WithData("SourcePoLineNo", asnDetail.SourcePoLineNo);
                }

                sourcePoLineId = poDetail.Id;
            }

            detailInfos.Add(new PurchaseReceiptDetailCreateInfo
            {
                Id = GuidGenerator.Create(),
                ProductId = x.ProductId,
                ProductCode = x.ProductCode,
                ProductName = x.ProductName,
                ExpectedQuantity = x.ExpectedQuantity,
                SourceAsnLineId = x.SourceAsnLineId,
                SourcePoLineId = sourcePoLineId,
                BatchNo = x.BatchNo
            });
        }

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
        return await BuildReceiptDtoAsync(persisted!);
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

        return await BuildReceiptDtoAsync(receipt);
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

    /// <summary>
    /// 向采购收货明细新增收货记录。
    /// </summary>
            public async Task<PurchaseRecordDto> AddRecordsAsync(AddPurchaseReceiptRecordsDto input)
    {
        if (input.PurchaseReceiptId == Guid.Empty)
        {
            throw new BusinessException("采购收货单Id不能为空。");
        }

        if (input.DetailId == Guid.Empty)
        {
            throw new BusinessException("采购收货明细Id不能为空。");
        }

        if (input.Records.Count != 1)
        {
            throw new BusinessException("一次仅允许新增一条收货记录。");
        }

        var recordInfos = new List<PurchaseRecordCreateInfo>(input.Records.Count);
        foreach (var record in input.Records)
        {
            var containerCode = record.ContainerCode?.Trim();
            if (string.IsNullOrWhiteSpace(containerCode))
            {
                throw new BusinessException("容器编码不能为空。");
            }

            var locationCode = record.LocationCode?.Trim();
            if (string.IsNullOrWhiteSpace(locationCode))
            {
                throw new BusinessException("库位编码不能为空。");
            }

            var container = await containerIntegrationService.GetContainerByCodeAsync(containerCode);
            if (container == null)
            {
                throw new BusinessException("系统不存在对应的容器编码。")
                    .WithData("ContainerCode", containerCode);
            }

            var location = await locationIntegrationService.GetLocationInfoByCodeAsync(locationCode);

            recordInfos.Add(new PurchaseRecordCreateInfo
            {
                Id = GuidGenerator.Create(),
                ReceivedQuantity = record.ReceivedQuantity,
                ContainerId = container.Id,
                ContainerCode = containerCode,
                LocationId = location.Id,
                LocationCode = locationCode,
                BatchNo = record.BatchNo,
                SupplierBatchNo = record.SupplierBatchNo
            });
        }

        // 第一步：调用领域服务追加收货记录并完成业务校验
        await purchaseReceiptManager.AddRecordsAsync(input.PurchaseReceiptId, input.DetailId, recordInfos);

        // 第二步：返回新增记录
        var createdRecordId = recordInfos[0].Id;
        var createdRecord = await purchaseRecordRepository.FirstOrDefaultAsync(x => x.Id == createdRecordId);
        if (createdRecord == null)
        {
            throw new EntityNotFoundException(typeof(PurchaseRecord), createdRecordId);
        }

        return new PurchaseRecordDto
        {
            Id = createdRecord.Id,
            PurchaseReceiptId = createdRecord.PurchaseReceiptId,
            PurchaseReceiptDetailId = createdRecord.PurchaseReceiptDetailId,
            SourceDetailId = createdRecord.SourceDetailId,
            ProductId = createdRecord.ProductId,
            ProductName = createdRecord.ProductName,
            ProductCode = createdRecord.ProductCode,
            ReceivedQuantity = createdRecord.ReceivedQuantity,
            ContainerId = createdRecord.ContainerId,
            ContainerCode = createdRecord.ContainerCode,
            LocationId = createdRecord.LocationId,
            LocationCode = createdRecord.LocationCode,
            BatchNo = createdRecord.BatchNo,
            SupplierBatchNo = createdRecord.SupplierBatchNo
        };
    }

    private async Task<PurchaseReceiptDto> BuildReceiptDtoAsync(PurchaseReceipt receipt)
    {
        var dto = ObjectMapper.Map<PurchaseReceipt, PurchaseReceiptDto>(receipt);

        var records = await AsyncExecuter.ToListAsync(
            (await purchaseRecordRepository.GetQueryableAsync())
            .Where(x => x.PurchaseReceiptId == receipt.Id));

        var recordDtoLookup = records
            .Select(r => new PurchaseRecordDto
            {
                Id = r.Id,
                PurchaseReceiptId = r.PurchaseReceiptId,
                PurchaseReceiptDetailId = r.PurchaseReceiptDetailId,
                SourceDetailId = r.SourceDetailId,
                ProductId = r.ProductId,
                ProductName = r.ProductName,
                ProductCode = r.ProductCode,
                ReceivedQuantity = r.ReceivedQuantity,
                ContainerId = r.ContainerId,
                ContainerCode = r.ContainerCode,
                LocationId = r.LocationId,
                LocationCode = r.LocationCode,
                BatchNo = r.BatchNo,
                SupplierBatchNo = r.SupplierBatchNo
            })
            .GroupBy(x => x.PurchaseReceiptDetailId)
            .ToDictionary(x => x.Key, x => x.ToList());

        foreach (var detail in dto.Details)
        {
            detail.Records = recordDtoLookup.TryGetValue(detail.Id, out var detailRecords)
                ? detailRecords
                : new List<PurchaseRecordDto>();
        }

        return dto;
    }
}



