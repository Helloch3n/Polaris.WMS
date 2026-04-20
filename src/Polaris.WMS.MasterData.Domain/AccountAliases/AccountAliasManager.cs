using Polaris.WMS.MasterData.AccountAliases;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Polaris.WMS.MasterData.Domain.AccountAliases;

public class AccountAliasManager(IAccountAliasRepository accountAliasRepository) : DomainService
{
    public async Task<AccountAlias> CreateAsync(
        string alias,
        string description,
        DateTime effectiveDate,
        DateTime? expireDate,
        bool isUnitPriceRequired,
        bool isProjectRequired,
        bool isDepartmentRequired,
        bool isProductionNoRequired,
        bool isWorkOrderOperationRequired,
        ProductionCostType productionCostType,
        bool isSupplierRequired,
        bool isCustomerRequired,
        bool isWorkOrderAttributeRequired)
    {
        if (expireDate.HasValue && expireDate.Value.Date < effectiveDate.Date)
        {
            throw new BusinessException("WMS:AccountAliasDateRangeInvalid")
                .WithData("EffectiveDate", effectiveDate)
                .WithData("ExpireDate", expireDate);
        }

        var existing = await accountAliasRepository.GetByAliasAsync(alias);
        if (existing != null)
        {
            throw new BusinessException("WMS:AccountAliasAlreadyExists")
                .WithData("Alias", alias);
        }

        var entity = new AccountAlias(
            GuidGenerator.Create(),
            alias,
            description,
            effectiveDate,
            expireDate,
            isUnitPriceRequired,
            isProjectRequired,
            isDepartmentRequired,
            isProductionNoRequired,
            isWorkOrderOperationRequired,
            productionCostType,
            isSupplierRequired,
            isCustomerRequired,
            isWorkOrderAttributeRequired);

        return await accountAliasRepository.InsertAsync(entity);
    }
}


