using Polaris.WMS.MasterData.Application.Contracts.Suppliers;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace Polaris.WMS.MasterData.Application.Suppliers;

[Mapper]
public partial class SupplierMappers : MapperBase<Supplier, SupplierDto>
{
    public override partial SupplierDto Map(Supplier source);
    public override partial void Map(Supplier source, SupplierDto destination);
}
