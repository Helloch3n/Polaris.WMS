using Volo.Abp.DependencyInjection;

namespace Polaris.WMS.MasterData.Domain.Integration.Identity.Users;

public interface IUserAllocationAdapter : ITransientDependency
{
    Task<List<ExternalUserInfo>> GetUserInfoAsync(List<Guid> userId);
}