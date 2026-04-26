using Volo.Abp.DependencyInjection;

namespace Polaris.WMS.Inbound.Domain.Integration.MasterData;

public interface IProductValidationProvider : ITransientDependency
{
    Task<bool> ExistsAsync(Guid productId);
}

