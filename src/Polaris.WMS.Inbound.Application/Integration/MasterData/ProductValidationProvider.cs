using Polaris.WMS.Inbound.Domain.Integration.MasterData;
using Polaris.WMS.MasterData.Application.Contracts.Integration.Products;
using Volo.Abp.DependencyInjection;

namespace Polaris.WMS.Inbound.Application.Integration.MasterData;

public class ProductValidationProvider(
    IProductIntegrationService productIntegrationService)
    : IProductValidationProvider, ITransientDependency
{
    public async Task<bool> ExistsAsync(Guid productId)
    {
        var products = await productIntegrationService.GetListByIdsAsync(new List<Guid> { productId });
        return products.Any();
    }
}

