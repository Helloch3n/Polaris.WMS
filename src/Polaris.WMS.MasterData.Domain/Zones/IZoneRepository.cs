using Volo.Abp.Domain.Repositories;

namespace Polaris.WMS.MasterData.Domain.Zones
{
    public interface IZoneRepository : IRepository<Zone, Guid>
    {
        Task<Zone?> GetByCodeAsync(string code);
        Task<List<Zone>> GetListByWarehouseIdAsync(Guid warehouseId);
    }
}

