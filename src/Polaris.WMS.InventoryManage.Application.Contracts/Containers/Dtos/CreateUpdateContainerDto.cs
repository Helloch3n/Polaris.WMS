using System.ComponentModel.DataAnnotations;
using Polaris.WMS.MasterData.Containers;

namespace Polaris.WMS.InventoryManage.Application.Contracts.Containers.Dtos
{
    public class CreateUpdateContainerDto
    {
        public string ContainerCode { get; set; }

        [Required]
        public string Name { get; set; }
        public ContainerType ContainerType { get; set; }
        public string Size { get; set; }
        public decimal SelfWeight { get; set; }
        public Guid? CurrentLocationId { get; set; }
    }
}
