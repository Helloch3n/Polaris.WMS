using Microsoft.EntityFrameworkCore;
using Polaris.WMS.MasterData.Domain.Locations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Polaris.WMS.MasterData.EntityFrameworkCore.Locations
{
    public class LocationRepository(IDbContextProvider<MasterDataDbContext> dbContextProvider)
        : EfCoreRepository<MasterDataDbContext, Location, Guid>(dbContextProvider), ILocationRepository
    {
        public async Task<Location?> GetByCodeAsync(string code)
        {
            var queryable = await GetQueryableAsync();
            var location = await queryable.FirstOrDefaultAsync(x => x.Code == code);
            if (location == null)
            {
                throw new UserFriendlyException($"未找到编码为 {code} 的库位！");
            }

            return location;
        }
        public async Task<Location> FindByCodeAsync(string code)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet.FirstOrDefaultAsync(x => x.Code == code);
        }

        public async Task<List<Location>> GetListByZoneIdAsync(Guid zoneId)
        {
            var queryable = await GetQueryableAsync();
            return await queryable.Where(x => x.ZoneId == zoneId).ToListAsync();
        }

        public async Task<List<Location>> GetListByWarehouseIdAsync(Guid warehouseId)
        {
            var queryable = await GetQueryableAsync();
            return await queryable.Where(x => x.WarehouseId == warehouseId).ToListAsync();
        }

        public async Task<string?> GetCodeByIdAsync(Guid id)
        {
            var queryable = await GetQueryableAsync();
            return await queryable
                .Where(x => x.Id == id)
                .Select(x => x.Code)
                .FirstOrDefaultAsync();
        }

        public async Task<Dictionary<Guid, string>> GetCodeMapByIdsAsync(List<Guid> ids)
        {
            if (ids.Count == 0)
            {
                return new Dictionary<Guid, string>();
            }

            var queryable = await GetQueryableAsync();
            return await queryable
                .Where(x => ids.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, x => x.Code);
        }
    }
}