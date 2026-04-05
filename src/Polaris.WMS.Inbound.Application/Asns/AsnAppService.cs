using Polaris.WMS.Inbound.Application.Contracts.Asns;
using Polaris.WMS.Inbound.Application.Contracts.Asns.Dtos;
using Polaris.WMS.Inbound.Domain.Asns;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Polaris.WMS.Inbound.Application.Asns;

public class AsnAppService(IRepository<AdvancedShippingNotice, Guid> repository) : ApplicationService, IAsnAppService
{
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