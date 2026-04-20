using Polaris.WMS.Inventories.Containers;
using Polaris.WMS.MasterData.Containers;
using Volo.Abp.Application.Dtos;

namespace Polaris.WMS.InventoryManage.Application.Contracts.Containers.Dtos
{
    public class ContainerDto : FullAuditedEntityDto<Guid>
    {
        public string ContainerCode { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public bool IsLocked { get; set; }
        public decimal SelfWeight { get; set; }
        public ContainerStatus Status { get; set; }
        public Guid? CurrentLocationId { get; set; }
        public string CurrentLocationCode { get; set; }
        public Guid? CurrentZoneId { get; set; }
        public string CurrentZoneCode { get; set; }
        public Guid? CurrentWarehouseId { get; set; }
        public string CurrentWarehouseCode { get; set; }
        public ContainerType containerType { get; set; }
    }
}
