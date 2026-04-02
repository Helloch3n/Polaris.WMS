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

    public async Task<ProductIntegrationDto> GetProductInfoByCodeAsync(string productCode)
    {
        var product = await productRepository.FirstOrDefaultAsync(x => x.Code == productCode);
        if (product == null)
        {
            throw new UserFriendlyException($"主数据异常：WMS 物料库中不存在代码为 [{productCode}] 的物料。请先同步主数据！");
        }

        return new ProductIntegrationDto { Id = product.Id, Name = product.Name, Code = product.Code };
    }
}