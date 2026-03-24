using Polaris.WMS.MasterData.Application.Contracts.Warehouses.Dtos;
using Polaris.WMS.MasterData.Domain.warehouses;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace Polaris.WMS.MasterData.Application.Warehouses;

[Mapper]
public partial class WarehouseMappers : MapperBase<Warehouse, WarehouseDto>
{
    public override partial WarehouseDto Map(Warehouse source);
    public override partial void Map(Warehouse source, WarehouseDto destination);
}
