using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;

namespace Polaris.WMS.Integration.Departments;

[RemoteService(IsEnabled = false)]
public interface IDepartmentIntegrationService : IApplicationService
{
    Task<DepartmentIntegrationDto> GetAsync(Guid id);
    
    Task<List<DepartmentIntegrationDto>> GetListByIdsAsync(List<Guid> ids);
}