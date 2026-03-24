using Microsoft.EntityFrameworkCore;
using Polaris.WMS.MasterData.Domain.Zones;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Polaris.WMS.MasterData.EntityFrameworkCore.Zones
{
    public class ZoneRepository(IDbContextProvider<MasterDataDbContext> dbContextProvider)
        : EfCoreRepository<MasterDataDbContext, Zone, Guid>(dbContextProvider), IZoneRepository
    {
        public async Task<Zone?> GetByCodeAsync(string code)
        {
            var queryable = await GetQueryableAsync();
            return await queryable.FirstOrDefaultAsync(x => x.Code == code);
        }

        public async Task<List<Zone>> GetListByWarehouseIdAsync(Guid warehouseId)
        {
            var queryable = await GetQueryableAsync();
            return await queryable.Where(x => x.WarehouseId == warehouseId).ToListAsync();
        }
    }
}

