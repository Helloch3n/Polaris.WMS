using Polaris.WMS.MasterData.AccountAliases;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Polaris.WMS.MasterData.Domain.AccountAliases;

public class AccountAlias : FullAuditedAggregateRoot<Guid>
{
    public string Alias { get; private set; }
    public string Description { get; private set; }
    public DateTime EffectiveDate { get; private set; }
    public DateTime? ExpireDate { get; private set; }
    public bool IsUnitPriceRequired { get; private set; }
    public bool IsProjectRequired { get; private set; }
    public bool IsDepartmentRequired { get; private set; }
    public bool IsProductionNoRequired { get; private set; }
    public bool IsWorkOrderOperationRequired { get; private set; }
    public ProductionCostType ProductionCostType { get; private set; }
    public bool IsSupplierRequired { get; private set; }
    public bool IsCustomerRequired { get; private set; }
    public bool IsWorkOrderAttributeRequired { get; private set; }

    protected AccountAlias()
    {
        Alias = string.Empty;
        Description = string.Empty;
    }

    internal AccountAlias(
        Guid id,
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
        bool isWorkOrderAttributeRequired) : base(id)
    {
        Update(
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
    }

    public void Update(
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
        Alias = Check.NotNullOrWhiteSpace(alias, nameof(alias));
        Description = description?.Trim() ?? string.Empty;
        EffectiveDate = effectiveDate;
        ExpireDate = expireDate;
        IsUnitPriceRequired = isUnitPriceRequired;
        IsProjectRequired = isProjectRequired;
        IsDepartmentRequired = isDepartmentRequired;
        IsProductionNoRequired = isProductionNoRequired;
        IsWorkOrderOperationRequired = isWorkOrderOperationRequired;
        ProductionCostType = productionCostType;
        IsSupplierRequired = isSupplierRequired;
        IsCustomerRequired = isCustomerRequired;
        IsWorkOrderAttributeRequired = isWorkOrderAttributeRequired;
    }
}

