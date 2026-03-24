using Volo.Abp.Application.Dtos;

namespace Polaris.WMS.MasterData.Application.Contracts.Warehouses.Dtos
{
    public class WarehouseDto : AuditedEntityDto<Guid>
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}

