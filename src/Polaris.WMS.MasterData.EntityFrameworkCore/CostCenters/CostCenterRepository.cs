using Microsoft.EntityFrameworkCore;
using Polaris.WMS.MasterData.Domain.CostCenters;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Polaris.WMS.MasterData.EntityFrameworkCore.CostCenters;

public class CostCenterRepository(IDbContextProvider<MasterDataDbContext> dbContextProvider)
    : EfCoreRepository<MasterDataDbContext, CostCenter, Guid>(dbContextProvider), ICostCenterRepository
{
    public async Task<CostCenter?> GetByCodeAsync(string code)
    {
        var queryable = await GetQueryableAsync();
        return await queryable.FirstOrDefaultAsync(x => x.Code == code);
    }
}


