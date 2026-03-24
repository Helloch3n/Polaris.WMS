using Polaris.WMS.InventoryManage.Application.Contracts.InventoryTransactions.Dtos;
using Polaris.WMS.InventoryManage.Domain.inventories;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace Polaris.WMS.InventoryManage.Application.InventoryTransactions
{
    [Mapper]
    public partial class InventoryMapper : MapperBase<InventoryTransaction, InventoryTransactionDto>
    {
        // 🌟 明确告诉编译器：跳过 DTO 中的这几个展示字段，我会用字典手动组装！
        // (请根据你 DTO 里实际存在的字段名进行删减)
        [MapperIgnoreTarget(nameof(InventoryTransactionDto.ReelNo))]
        [MapperIgnoreTarget(nameof(InventoryTransactionDto.ProductName))]
        [MapperIgnoreTarget(nameof(InventoryTransactionDto.FromLocationCode))]
        [MapperIgnoreTarget(nameof(InventoryTransactionDto.ToLocationCode))]
        [MapperIgnoreTarget(nameof(InventoryTransactionDto.FromWarehouseCode))]
        [MapperIgnoreTarget(nameof(InventoryTransactionDto.ToWarehouseCode))]
        public override partial InventoryTransactionDto Map(InventoryTransaction source);

        public override partial void Map(InventoryTransaction source, InventoryTransactionDto destination);
    }
}