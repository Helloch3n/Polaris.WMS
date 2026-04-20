using Microsoft.EntityFrameworkCore;
using Polaris.WMS.InventoryManage.Domain.Containers;
using Polaris.WMS.MasterData.Containers;
using Polaris.WMS.MasterData.Warehouses;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Polaris.WMS.InventoryManage.EntityFrameworkCore.Containers
{
    // public class ContainerRepository(IDbContextProvider<InventoryDbContext> dbContextProvider)
    //     : EfCoreRepository<InventoryDbContext, Container, Guid>(dbContextProvider),
    //         IContainerRepository
    // {
    //     public async Task<List<Container>> GetAvailableForPutawayListAsync(
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
    //             from container in queryable
    //             join location in dbContext.Locations on container.CurrentLocationId equals location.Id
    //             join zone in dbContext.Zones on location.ZoneId equals zone.Id
    //             where container.Status == ContainerStatus.Occupied
    //             where !container.IsLocked
    //             where zone.ZoneType == ZoneType.Dock
    //             select new { container, location };
    //
    //         if (!string.IsNullOrWhiteSpace(filter))
    //         {
    //             query = query.Where(x => x.container.ContainerCode.Contains(filter));
    //         }
    //
    //         if (warehouseId.HasValue)
    //         {
    //             query = query.Where(x => x.location.WarehouseId == warehouseId.Value);
    //         }
    //
    //         var sortingValue = string.IsNullOrWhiteSpace(sorting)
    //             ? nameof(Container.ContainerCode)
    //             : sorting;
    //
    //         return await query
    //             .OrderBy($"container.{sortingValue}")
    //             .Skip(skipCount)
    //             .Take(maxResultCount)
    //             .Select(x => x.container)
    //             .ToListAsync();
    //     }
    //
    //     public async Task<long> GetAvailableForPutawayCountAsync(string filter, Guid? warehouseId)
    //     {
    //         var queryable = await GetQueryableAsync();
    //         var dbContext = await GetDbContextAsync();
    //
    //         var query =
    //             from container in queryable
    //             join location in dbContext.Locations on container.CurrentLocationId equals location.Id
    //             join zone in dbContext.Zones on location.ZoneId equals zone.Id
    //             where container.Status == ContainerStatus.Occupied
    //             where !container.IsLocked
    //             where zone.ZoneType == ZoneType.Dock
    //             select new { container, location };
    //
    //         if (!string.IsNullOrWhiteSpace(filter))
    //         {
    //             query = query.Where(x => x.container.ContainerCode.Contains(filter));
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
    //     public async Task<Container?> GetByContainerCodeWithLocationAsync(string containerCode)
    //     {
    //         var queryable = await GetQueryableAsync();
    //         return await queryable.FirstOrDefaultAsync(x => x.ContainerCode == containerCode);
    //     }
    // }
}
