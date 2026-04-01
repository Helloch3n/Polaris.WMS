using Polaris.WMS.MasterData.Application.Contracts.Products;
using Polaris.WMS.MasterData.Application.Contracts.Products.Dtos;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace Polaris.WMS.MasterData.Application.Products;

[Mapper]
public partial class ProductMappers : MapperBase<Product, ProductDto>
{
    public override partial ProductDto Map(Product source);
    public override partial void Map(Product source, ProductDto destination);
}
