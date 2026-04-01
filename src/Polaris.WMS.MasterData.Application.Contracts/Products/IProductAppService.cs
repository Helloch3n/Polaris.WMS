using Polaris.WMS.MasterData.Application.Contracts.Products.Dtos;
using Volo.Abp.Application.Services;

namespace Polaris.WMS.MasterData.Application.Contracts.Products
{
    public interface IProductAppService : ICrudAppService<ProductDto, Guid, ProductSearchDto, CreateUpdateProductDto>
    {
    }
}
