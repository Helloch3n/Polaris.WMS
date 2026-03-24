using Polaris.WMS.InventoryManage.Application.Contracts.Inventories.Dtos;
using Polaris.WMS.InventoryManage.Domain.inventories;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace Polaris.WMS.InventoryManage.Application.Inventories
{
    [Mapper]
    public partial class InventoryMappers : MapperBase<Inventory, InventoryDto>
    {
        [MapProperty(nameof(Inventory.Index), nameof(InventoryDto.LayerIndex))]
        public override partial InventoryDto Map(Inventory source);

        public override partial void Map(Inventory source, InventoryDto destination);
    }
}