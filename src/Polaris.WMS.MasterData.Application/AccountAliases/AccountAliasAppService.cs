using Polaris.WMS.MasterData.Application.Contracts.AccountAliases;
using Polaris.WMS.MasterData.Application.Contracts.AccountAliases.Dtos;
using Polaris.WMS.MasterData.Domain.AccountAliases;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Polaris.WMS.MasterData.Application.AccountAliases;

public class AccountAliasAppService(
    IRepository<AccountAlias, Guid> accountAliasRepository,
    AccountAliasManager accountAliasManager)
    : ApplicationService, IAccountAliasAppService
{
    /// <inheritdoc />
    public async Task<AccountAliasDto> CreateAsync(CreateAccountAliasDto input)
    {
        if (input.ExpireDate.HasValue && input.ExpireDate.Value.Date < input.EffectiveDate.Date)
        {
            throw new BusinessException("WMS:AccountAliasDateRangeInvalid");
        }

        var entity = await accountAliasManager.CreateAsync(
            input.Alias,
            input.Description,
            input.EffectiveDate,
            input.ExpireDate,
            input.IsUnitPriceRequired,
            input.IsProjectRequired,
            input.IsDepartmentRequired,
            input.IsProductionNoRequired,
            input.IsWorkOrderOperationRequired,
            input.ProductionCostType,
            input.IsSupplierRequired,
            input.IsCustomerRequired,
            input.IsWorkOrderAttributeRequired);

        return ObjectMapper.Map<AccountAlias, AccountAliasDto>(entity);
    }

    /// <inheritdoc />
    public async Task<AccountAliasDto> GetAsync(Guid id)
    {
        var entity = await accountAliasRepository.GetAsync(id);
        return ObjectMapper.Map<AccountAlias, AccountAliasDto>(entity);
    }

    /// <inheritdoc />
    public async Task<PagedResultDto<AccountAliasDto>> GetListAsync(AccountAliasSearchDto input)
    {
        var query = await accountAliasRepository.GetQueryableAsync();

        if (!string.IsNullOrWhiteSpace(input.Alias))
        {
            query = query.Where(x => x.Alias.Contains(input.Alias));
        }

        if (!string.IsNullOrWhiteSpace(input.Description))
        {
            query = query.Where(x => x.Description.Contains(input.Description));
        }

        if (input.IsActiveAtNow == true)
        {
            var now = Clock.Now.Date;
            query = query.Where(x => x.EffectiveDate.Date <= now && (!x.ExpireDate.HasValue || x.ExpireDate.Value.Date >= now));
        }

        var totalCount = await AsyncExecuter.CountAsync(query);

        query = query
            .OrderBy(x => x.Alias)
            .PageBy(input.SkipCount, input.MaxResultCount);

        var entities = await AsyncExecuter.ToListAsync(query);
        var items = entities.Select(ObjectMapper.Map<AccountAlias, AccountAliasDto>).ToList();

        return new PagedResultDto<AccountAliasDto>(totalCount, items);
    }
}

