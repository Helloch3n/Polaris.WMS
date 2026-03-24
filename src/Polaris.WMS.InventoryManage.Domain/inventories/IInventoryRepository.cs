using Volo.Abp.Domain.Repositories;

namespace Polaris.WMS.InventoryManage.Domain.inventories
{
    public interface IInventoryRepository : IRepository<Inventory, Guid>
    {
        Task<int> GetMaxLayerOnReelAsync(Guid reelId);
        //Task<List<Inventory>> GetAllocatableInventoriesAsync(string productCode, decimal targetLength);
        Task<bool> IsSnExistsAsync(string sn);
    }
}
