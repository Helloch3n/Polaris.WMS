using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Polaris.WMS.InventoryManage.Domain.inventories;
using Polaris.WMS.InventoryManage.Domain.Containers;
using Polaris.WMS.MasterData.Containers;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Polaris.WMS.InventoryManage.EntityFrameworkCore.Inventories
{
    public class InventoryRepository(IDbContextProvider<InventoryDbContext> dbContextProvider)
        : EfCoreRepository<InventoryDbContext, Inventory, Guid>(dbContextProvider), IInventoryRepository
    {
        public async Task<int> GetMaxLayerOnReelAsync(Guid containerId)
        {
            return await (await GetDbSetAsync())
                .Where(x => x.ContainerId == containerId)
                .MaxAsync(x => (int?)x.LayerIndex) ?? 0;
        }

        // public async Task<List<Inventory>> GetAllocatableInventoriesAsync(string productCode, decimal targetLength)
        // {
        //     var dbContext = await GetDbContextAsync();
        //     var inventories = dbContext.Set<Inventory>();
        //     var reels = dbContext.Set<Container>();
        //     var products = dbContext.Set<Product>();
        //
        //     var candidates = await (
        //         from inventory in inventories
        //         join container in reels on inventory.ContainerId equals container.Id
        //         join product in products on inventory.ProductId equals product.Id
        //         where product.Code == productCode
        //         where container.Status == ContainerStatus.Occupied
        //         where !container.IsLocked
        //         select inventory)
        //         .ToListAsync();
        //
        //     return candidates
        //         .GroupBy(x => x.ContainerId)
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