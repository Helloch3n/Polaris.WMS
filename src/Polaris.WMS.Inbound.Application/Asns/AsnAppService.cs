using System.Linq.Dynamic.Core;
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

    public async Task<PagedResultDto<AdvancedShippingNoticeDto>> GetListAsync(GetAsnListDto input)
    {
        var query = (await repository.GetQueryableAsync())
            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), 
                x => x.AsnNo.Contains(input.Filter) || x.SupplierName.Contains(input.Filter))
            .WhereIf(input.Status.HasValue, x => x.Status == input.Status)
            .WhereIf(!string.IsNullOrWhiteSpace(input.LicensePlate), 
                x => x.Details.Any(d => d.LicensePlate.Contains(input.LicensePlate))); // 使用 Any 进行子表车牌查询

        var totalCount = await AsyncExecuter.CountAsync(query);

        var items = await AsyncExecuter.ToListAsync(
            query.OrderBy(string.IsNullOrWhiteSpace(input.Sorting) ? "CreationTime DESC" : input.Sorting)
                 .Skip(input.SkipCount)
                 .Take(input.MaxResultCount)
        );

        return new PagedResultDto<AdvancedShippingNoticeDto>(
            totalCount,
            ObjectMapper.Map<List<AdvancedShippingNotice>, List<AdvancedShippingNoticeDto>>(items)
        );
    }
}