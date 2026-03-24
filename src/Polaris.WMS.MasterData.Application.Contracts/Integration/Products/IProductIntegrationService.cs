using Volo.Abp;
using Volo.Abp.Application.Services;

namespace Polaris.WMS.MasterData.Application.Contracts.Integration.Products;

[RemoteService(IsEnabled = false)]
public interface IProductIntegrationService : IApplicationService
{
    Task<List<ProductIntegrationDto>> GetListByIdsAsync(List<Guid> ids);
}