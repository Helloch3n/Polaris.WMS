using Microsoft.EntityFrameworkCore;
using Polaris.WMS.MasterData.Domain.AccountAliases;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Polaris.WMS.MasterData.EntityFrameworkCore.AccountAliases;

public class AccountAliasRepository(IDbContextProvider<MasterDataDbContext> dbContextProvider)
    : EfCoreRepository<MasterDataDbContext, AccountAlias, Guid>(dbContextProvider), IAccountAliasRepository
{
    public async Task<AccountAlias?> GetByAliasAsync(string alias)
    {
        var queryable = await GetQueryableAsync();
        return await queryable.FirstOrDefaultAsync(x => x.Alias == alias);
    }
}

