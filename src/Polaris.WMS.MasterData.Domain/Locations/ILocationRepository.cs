using Volo.Abp.Domain.Repositories;

namespace Polaris.WMS.MasterData.Domain.Locations
{
    public interface ILocationRepository : IRepository<Location, Guid>
    {
        Task<Location?> GetByCodeAsync(string code);
        Task<List<Location>> GetListByZoneIdAsync(Guid zoneId);
        Task<List<Location>> GetListByWarehouseIdAsync(Guid warehouseId);
        Task<string?> GetCodeByIdAsync(Guid id);
        Task<Dictionary<Guid, string>> GetCodeMapByIdsAsync(List<Guid> ids);
    }
}


