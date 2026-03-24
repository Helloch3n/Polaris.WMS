using Polaris.WMS.MasterData.Domain.Integration.Identity.Users;
using Volo.Abp;

namespace Polaris.WMS.MasterData.Application.Integration.Identity;

[RemoteService(IsEnabled = false)]
public class UserAllocationAdapter:IUserAllocationAdapter
{
    public Task<List<ExternalUserInfo>> GetUserInfoAsync(List<Guid> userId)
    {
        throw new NotImplementedException();
    }
}