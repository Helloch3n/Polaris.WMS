using Polaris.WMS.MasterData.Application.Contracts.AccountAliases.Dtos;
using Polaris.WMS.MasterData.Domain.AccountAliases;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace Polaris.WMS.MasterData.Application.AccountAliases;

[Mapper]
public partial class AccountAliasMappers : MapperBase<AccountAlias, AccountAliasDto>
{
    public override partial AccountAliasDto Map(AccountAlias source);
    public override partial void Map(AccountAlias source, AccountAliasDto destination);
}

