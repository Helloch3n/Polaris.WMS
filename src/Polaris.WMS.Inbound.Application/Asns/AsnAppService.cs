using Polaris.WMS.Inbound.Application.Contracts.Asns;
using Polaris.WMS.Inbound.Application.Contracts.Asns.Dtos;
using Polaris.WMS.Inbound.Application.Contracts.DataSync.Dtos;
using Polaris.WMS.Inbound.Domain.Asns;
using Polaris.WMS.Inbound.Domain.PurchaseOrders;
using Polaris.WMS.MasterData.Application.Contracts.Integration.Products;
using Polaris.WMS.MasterData.Application.Contracts.Integration.Suppliers;
using Polaris.WMS.Asns;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Polaris.WMS.Inbound.Application.Asns;

public class AsnAppService(
    IRepository<AdvancedShippingNotice, Guid> repository,
    IRepository<PurchaseOrder, Guid> purchaseOrderRepository,
    ISupplierIntegrationService supplierIntegrationService,
    IProductIntegrationService productIntegrationService)
    : ApplicationService, IAsnAppService
{
    public async Task SyncAsnAsync(SyncAsnDto input)
    {
        var existingAsn = await repository.FirstOrDefaultAsync(x => x.AsnNo == input.AsnNo);
        var supplierInfo = await supplierIntegrationService.GetSupplierInfoByCodeAsync(input.SupplierCode);

        if (existingAsn == null)
        {
            var newAsn = new AdvancedShippingNotice(
                GuidGenerator.Create(),
                input.AsnNo,
                supplierInfo.Id,
                input.SupplierCode,
                supplierInfo.Name,
                input.ExpectedArrivalTime);

            foreach (var item in input.Details)
            {
                var po = await purchaseOrderRepository.GetAsync(x => x.PoNo == item.SourcePoNo);
                if (po == null)
                {
                    throw new UserFriendlyException($"ASN 明细行 {item.ScmAsnRowNo} 中的源头采购单号 {item.SourcePoNo} 在系统中不存在！");
                }

                var productInfo = await productIntegrationService.GetProductInfoByCodeAsync(item.ProductCode);
                newAsn.AddDetail(
                    GuidGenerator.Create(),
                    item.ScmAsnRowNo,
                    po.Id,
                    item.SourcePoNo,
                    item.SourcePoLineNo,
                    productInfo.Id,
                    item.ProductCode,
                    productInfo.Name,
                    item.UoM,
                    item.ExpectedQty,
                    item.SupplierBatchNo,
                    input.LicensePlate);
            }

            await repository.InsertAsync(newAsn);
            return;
        }

        if (existingAsn.Status != AsnStatus.Pending)
        {
            throw new UserFriendlyException(
                $"ASN [{input.AsnNo}] 的送货车辆已到达月台并开始收货 (状态:{existingAsn.Status})。禁止 SCM 远程修改！请联系工厂仓管进行人工异常处理。");
        }

        existingAsn.UpdateBasicInfo(input.ExpectedArrivalTime);

        foreach (var item in input.Details)
        {
            var existingDetail = existingAsn.Details.FirstOrDefault(x => x.ScmAsnRowNo == item.ScmAsnRowNo);
            if (existingDetail == null)
            {
                var po = await purchaseOrderRepository.GetAsync(x => x.PoNo == item.SourcePoNo);
                if (po == null)
                {
                    throw new UserFriendlyException($"ASN 明细行 {item.ScmAsnRowNo} 中的源头采购单号 {item.SourcePoNo} 在系统中不存在！");
                }

                var productInfo = await productIntegrationService.GetProductInfoByCodeAsync(item.ProductCode);
                existingAsn.AddDetail(
                    GuidGenerator.Create(),
                    item.ScmAsnRowNo,
                    po.Id,
                    item.SourcePoNo,
                    item.SourcePoLineNo,
                    productInfo.Id,
                    item.ProductCode,
                    productInfo.Name,
                    item.UoM,
                    item.ExpectedQty,
                    item.SupplierBatchNo,
                    input.LicensePlate);
            }
            else
            {
                existingAsn.UpdateDetailInfo(
                    item.ScmAsnRowNo,
                    item.ExpectedQty,
                    item.SupplierBatchNo,
                    input.LicensePlate);
            }
        }

        var incomingRowNos = input.Details.Select(x => x.ScmAsnRowNo).ToList();
        existingAsn.RemoveDetailsNotIn(incomingRowNos);
        await repository.UpdateAsync(existingAsn);
    }

    public async Task<AdvancedShippingNoticeDto> GetAsync(Guid id)
    {
        var queryable = await repository.WithDetailsAsync(x => x.Details);
        var entity = await AsyncExecuter.FirstOrDefaultAsync(queryable.Where(x => x.Id == id));

        if (entity == null)
        {
            throw new EntityNotFoundException(typeof(AdvancedShippingNotice), id);
        }

        return ObjectMapper.Map<AdvancedShippingNotice, AdvancedShippingNoticeDto>(entity);
    }

    public async Task<AdvancedShippingNoticeDto> GetByAsnNoAsync(string asnNo)
    {
        var queryable = await repository.WithDetailsAsync(x => x.Details);
        var entity = await AsyncExecuter.FirstOrDefaultAsync(queryable.Where(x => x.AsnNo == asnNo));

        if (entity == null)
        {
            throw new UserFriendlyException($"未找到发货通知单号为 {asnNo} 的单据。");
        }

        return ObjectMapper.Map<AdvancedShippingNotice, AdvancedShippingNoticeDto>(entity);
    }

    public async Task<PagedResultDto<AdvancedShippingNoticeDto>> GetListAsync(AsnSearchDto input)
    {
        input ??= new AsnSearchDto();
        var asnNo = input.AsnNo?.Trim();
        var supplierName = input.SupplierName?.Trim();
        var licensePlate = input.LicensePlate?.Trim();

        var query = (await repository.GetQueryableAsync())
            .WhereIf(!string.IsNullOrWhiteSpace(asnNo), x => x.AsnNo == asnNo)
            .WhereIf(!string.IsNullOrWhiteSpace(supplierName), x => x.SupplierName.Contains(supplierName!))
            .WhereIf(input.Status.HasValue, x => x.Status == input.Status)
            .WhereIf(!string.IsNullOrWhiteSpace(licensePlate), x => x.Details.Any(d => d.LicensePlate.Contains(licensePlate!)));

        var totalCount = await AsyncExecuter.CountAsync(query);

        var sortingField = input.Sorting?.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        var isDesc = input.Sorting?.Contains("DESC", StringComparison.OrdinalIgnoreCase) == true;

        query = sortingField switch
        {
            nameof(AdvancedShippingNotice.AsnNo) => isDesc ? query.OrderByDescending(x => x.AsnNo) : query.OrderBy(x => x.AsnNo),
            nameof(AdvancedShippingNotice.SupplierName) => isDesc ? query.OrderByDescending(x => x.SupplierName) : query.OrderBy(x => x.SupplierName),
            nameof(AdvancedShippingNotice.Status) => isDesc ? query.OrderByDescending(x => x.Status) : query.OrderBy(x => x.Status),
            nameof(AdvancedShippingNotice.CreationTime) => isDesc ? query.OrderByDescending(x => x.CreationTime) : query.OrderBy(x => x.CreationTime),
            _ => query.OrderByDescending(x => x.CreationTime)
        };

        var skipCount = input.SkipCount < 0 ? 0 : input.SkipCount;
        var maxResultCount = input.MaxResultCount <= 0 ? 10 : input.MaxResultCount;

        var items = await AsyncExecuter.ToListAsync(
            query.Skip(skipCount)
                .Take(maxResultCount)
        );

        // return new PagedResultDto<AdvancedShippingNoticeDto>(
        //     totalCount,
        //     ObjectMapper.Map<List<AdvancedShippingNotice>, List<AdvancedShippingNoticeDto>>(items)
        // );
        return new PagedResultDto<AdvancedShippingNoticeDto>(
            totalCount,
            items.Select(x => ObjectMapper.Map<AdvancedShippingNotice, AdvancedShippingNoticeDto>(x)).ToList()
        );
    }
}