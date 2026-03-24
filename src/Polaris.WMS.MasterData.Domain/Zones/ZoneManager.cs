using Polaris.WMS.MasterData.Domain.warehouses;
using Polaris.WMS.MasterData.Warehouses;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Polaris.WMS.MasterData.Domain.Zones
{
    public class ZoneManager : DomainService
    {
        private readonly IZoneRepository _zoneRepository;
        private readonly IRepository<Warehouse, Guid> _warehouseRepository;

        public ZoneManager(
            IZoneRepository zoneRepository,
            IRepository<Warehouse, Guid> warehouseRepository)
        {
            _zoneRepository = zoneRepository;
            _warehouseRepository = warehouseRepository;
        }

        public async Task<Zone> CreateAsync(
            Guid warehouseId,
            string code,
            string name,
            ZoneType zoneType)
        {
            var warehouse = await _warehouseRepository.GetAsync(warehouseId);
            if (warehouse == null)
            {
                throw new BusinessException("WMS:WarehouseNotFound")
                    .WithData("WarehouseId", warehouseId);
            }

            var existing = await _zoneRepository.GetByCodeAsync(code);
            if (existing != null && existing.WarehouseId == warehouseId)
            {
                throw new BusinessException("WMS:ZoneCodeAlreadyExists")
                    .WithData("Code", code)
                    .WithData("WarehouseId", warehouseId);
            }

            var zone = new Zone(
                GuidGenerator.Create(),
                warehouseId,
                code,
                name,
                zoneType);

            await _zoneRepository.InsertAsync(zone);
            return zone;
        }
    }
}

