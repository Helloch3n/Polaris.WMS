using Polaris.WMS.Inventories.Reels;
using Polaris.WMS.MasterData.Reels;
using Volo.Abp.Application.Dtos;

namespace Polaris.WMS.InventoryManage.Application.Contracts.Reels.Dtos
{
    public class ReelDto : FullAuditedEntityDto<Guid>
    {
        public string ReelNo { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public bool IsLocked { get; set; }
        public decimal SelfWeight { get; set; }
        public ReelStatus Status { get; set; }
        public Guid? CurrentLocationId { get; set; }
        public string CurrentLocationCode { get; set; }
        public Guid? CurrentZoneId { get; set; }
        public string CurrentZoneCode { get; set; }
        public Guid? CurrentWarehouseId { get; set; }
        public string CurrentWarehouseCode { get; set; }
        public ReelType reelType { get; set; }
    }
}
