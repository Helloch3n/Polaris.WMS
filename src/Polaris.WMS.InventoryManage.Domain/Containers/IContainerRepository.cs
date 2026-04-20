using Volo.Abp.Domain.Repositories;

namespace Polaris.WMS.InventoryManage.Domain.Containers
{
    public interface IContainerRepository : IRepository<Container, Guid>
    {
        Task<List<Container>> GetAvailableForPutawayListAsync(
            string filter,
            Guid? warehouseId,
            string sorting,
            int skipCount,
            int maxResultCount);

        Task<long> GetAvailableForPutawayCountAsync(string filter, Guid? warehouseId);

        Task<Container?> GetByContainerCodeWithLocationAsync(string containerCode);
    }
}
