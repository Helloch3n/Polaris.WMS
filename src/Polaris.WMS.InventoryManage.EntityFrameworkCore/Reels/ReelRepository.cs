using Microsoft.EntityFrameworkCore;
using Polaris.WMS.InventoryManage.Domain.Reels;
using Polaris.WMS.MasterData.Reels;
using Polaris.WMS.MasterData.Warehouses;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Polaris.WMS.InventoryManage.EntityFrameworkCore.Reels
{
    // public class ReelRepository(IDbContextProvider<InventoryDbContext> dbContextProvider)
    //     : EfCoreRepository<InventoryDbContext, Reel, Guid>(dbContextProvider),
    //         IReelRepository
    // {
    //     public async Task<List<Reel>> GetAvailableForPutawayListAsync(
    //         string filter,
    //         Guid? warehouseId,
    //         string sorting,
    //         int skipCount,
    //         int maxResultCount)
    //     {
    //         var queryable = await GetQueryableAsync();
    //         var dbContext = await GetDbContextAsync();
    //
    //         var query =
    //             from reel in queryable
    //             join location in dbContext.Locations on reel.CurrentLocationId equals location.Id
    //             join zone in dbContext.Zones on location.ZoneId equals zone.Id
    //             where reel.Status == ReelStatus.Occupied
    //             where !reel.IsLocked
    //             where zone.ZoneType == ZoneType.Dock
    //             select new { reel, location };
    //
    //         if (!string.IsNullOrWhiteSpace(filter))
    //         {
    //             query = query.Where(x => x.reel.ReelNo.Contains(filter));
    //         }
    //
    //         if (warehouseId.HasValue)
    //         {
    //             query = query.Where(x => x.location.WarehouseId == warehouseId.Value);
    //         }
    //
    //         var sortingValue = string.IsNullOrWhiteSpace(sorting)
    //             ? nameof(Reel.ReelNo)
    //             : sorting;
    //
    //         return await query
    //             .OrderBy($"reel.{sortingValue}")
    //             .Skip(skipCount)
    //             .Take(maxResultCount)
    //             .Select(x => x.reel)
    //             .ToListAsync();
    //     }
    //
    //     public async Task<long> GetAvailableForPutawayCountAsync(string filter, Guid? warehouseId)
    //     {
    //         var queryable = await GetQueryableAsync();
    //         var dbContext = await GetDbContextAsync();
    //
    //         var query =
    //             from reel in queryable
    //             join location in dbContext.Locations on reel.CurrentLocationId equals location.Id
    //             join zone in dbContext.Zones on location.ZoneId equals zone.Id
    //             where reel.Status == ReelStatus.Occupied
    //             where !reel.IsLocked
    //             where zone.ZoneType == ZoneType.Dock
    //             select new { reel, location };
    //
    //         if (!string.IsNullOrWhiteSpace(filter))
    //         {
    //             query = query.Where(x => x.reel.ReelNo.Contains(filter));
    //         }
    //
    //         if (warehouseId.HasValue)
    //         {
    //             query = query.Where(x => x.location.WarehouseId == warehouseId.Value);
    //         }
    //
    //         return await query.LongCountAsync();
    //     }
    //
    //     public async Task<Reel?> GetByReelNoWithLocationAsync(string reelNo)
    //     {
    //         var queryable = await GetQueryableAsync();
    //         return await queryable.FirstOrDefaultAsync(x => x.ReelNo == reelNo);
    //     }
    // }
}
