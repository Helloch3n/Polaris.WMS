using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Polaris.WMS.InventoryManage.Domain.inventories;
using Polaris.WMS.InventoryManage.Domain.Reels;
using Polaris.WMS.MasterData.Reels;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Polaris.WMS.InventoryManage.EntityFrameworkCore.Inventories
{
    public class InventoryRepository(IDbContextProvider<InventoryDbContext> dbContextProvider)
        : EfCoreRepository<InventoryDbContext, Inventory, Guid>(dbContextProvider), IInventoryRepository
    {
        public async Task<int> GetMaxLayerOnReelAsync(Guid reelId)
        {
            return await (await GetDbSetAsync())
                .Where(x => x.ReelId == reelId)
                .MaxAsync(x => (int?)x.Index) ?? 0;
        }

        // public async Task<List<Inventory>> GetAllocatableInventoriesAsync(string productCode, decimal targetLength)
        // {
        //     var dbContext = await GetDbContextAsync();
        //     var inventories = dbContext.Set<Inventory>();
        //     var reels = dbContext.Set<Reel>();
        //     var products = dbContext.Set<Product>();
        //
        //     var candidates = await (
        //         from inventory in inventories
        //         join reel in reels on inventory.ReelId equals reel.Id
        //         join product in products on inventory.ProductId equals product.Id
        //         where product.Code == productCode
        //         where reel.Status == ReelStatus.Occupied
        //         where !reel.IsLocked
        //         select inventory)
        //         .ToListAsync();
        //
        //     return candidates
        //         .GroupBy(x => x.ReelId)
        //         .Select(g => g.OrderByDescending(x => x.Index).First())
        //         .Where(x => x.AvailableQuantity >= targetLength)
        //         .OrderBy(x => x.CreationTime)
        //         .ToList();
        // }

        public async Task<bool> IsSnExistsAsync(string sn)
        {
            return await (await GetDbSetAsync()).AnyAsync(x => x.SN == sn);
        }
    }
}