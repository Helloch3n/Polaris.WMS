using System.ComponentModel.DataAnnotations;
using Polaris.WMS.MasterData.Reels;

namespace Polaris.WMS.InventoryManage.Application.Contracts.Reels.Dtos
{
    public class CreateUpdateReelDto
    {
        public string ReelNo { get; set; }

        [Required]
        public string Name { get; set; }
        public ReelType ReelType { get; set; }
        public string Size { get; set; }
        public decimal SelfWeight { get; set; }
        public Guid? CurrentLocationId { get; set; }
    }
}
