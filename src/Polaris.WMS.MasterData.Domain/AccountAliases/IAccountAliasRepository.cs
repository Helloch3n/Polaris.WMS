using Volo.Abp.Domain.Repositories;

namespace Polaris.WMS.MasterData.Domain.AccountAliases;

public interface IAccountAliasRepository : IRepository<AccountAlias, Guid>
{
    Task<AccountAlias?> GetByAliasAsync(string alias);
}

