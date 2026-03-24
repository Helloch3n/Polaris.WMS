using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Polaris.WMS.Integration;

public interface IUserIntegrationService
{
    public Task<List<UserIntegrationDto>> GetUserProfileAsync(List<Guid> userId);
}