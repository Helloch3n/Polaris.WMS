using Polaris.WMS.MasterData.Application.Contracts.Integration.Products;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Polaris.WMS.MasterData.Application.Integration.Products;

[RemoteService(IsEnabled = false)]
public class ProductIntegrationService(
    IRepository<Product, Guid> productRepository 
) : ApplicationService, IProductIntegrationService
{
    public async Task<List<ProductIntegrationDto>> GetListByIdsAsync(List<Guid> ids)
    {
        if (ids == null || !ids.Any()) return new List<ProductIntegrationDto>();

        var query = await productRepository.GetQueryableAsync();
        var products = await AsyncExecuter.ToListAsync(query.Where(x => ids.Contains(x.Id)));

        return products.Select(p => new ProductIntegrationDto
        {
            Id = p.Id,
            Name = p.Name,
            Code = p.Code
        }).ToList();
    }
}