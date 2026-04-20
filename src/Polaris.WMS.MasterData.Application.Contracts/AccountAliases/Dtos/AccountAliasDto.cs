using Polaris.WMS.MasterData.AccountAliases;
using Volo.Abp.Application.Dtos;

namespace Polaris.WMS.MasterData.Application.Contracts.AccountAliases.Dtos;

public class AccountAliasDto : AuditedEntityDto<Guid>
{
    public string Alias { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime EffectiveDate { get; set; }
    public DateTime? ExpireDate { get; set; }
    public bool IsUnitPriceRequired { get; set; }
    public bool IsProjectRequired { get; set; }
    public bool IsDepartmentRequired { get; set; }
    public bool IsProductionNoRequired { get; set; }
    public bool IsWorkOrderOperationRequired { get; set; }
    public ProductionCostType ProductionCostType { get; set; }
    public bool IsSupplierRequired { get; set; }
    public bool IsCustomerRequired { get; set; }
    public bool IsWorkOrderAttributeRequired { get; set; }
}

