using Volo.Abp.Application.Services;

namespace Polaris.WMS.MasterData.Application.Contracts.Integration.Suppliers;

public interface ISupplierIntegrationService : IApplicationService
{
    Task<SupplierIntegrationDto> GetSupplierInfoByCodeAsync(string supplierCode);
}