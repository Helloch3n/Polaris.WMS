using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Identity;
using Volo.Abp.Domain.Repositories;

namespace Polaris.WMS.Integration.Departments;

[RemoteService(IsEnabled = false)]
public class DepartmentIntegrationService(
    IOrganizationUnitRepository organizationUnitRepository
) : ApplicationService, IDepartmentIntegrationService
{
    public async Task<DepartmentIntegrationDto> GetAsync(Guid id)
    {
        // FindAsync 找不到会返回 null，比较安全
        var ou = await organizationUnitRepository.FindAsync(id);
        if (ou == null) return null;

        return new DepartmentIntegrationDto
        {
            Id = ou.Id,
            DisplayName = ou.DisplayName,
            Code = ou.Code
        };
    }

    public async Task<List<DepartmentIntegrationDto>> GetListByIdsAsync(List<Guid> ids)
    {
        // 1. 防护拦截：空集合直接返回
        if (ids == null || !ids.Any()) return new List<DepartmentIntegrationDto>();

        //  2. 直接使用自带的 GetListAsync，传入 IN 查询条件
        var list = await organizationUnitRepository.GetListAsync(ids);

        // 3. 内存映射返回
        return list.Select(x => new DepartmentIntegrationDto
        {
            Id = x.Id,
            DisplayName = x.DisplayName,
            Code = x.Code
        }).ToList();
    }
}