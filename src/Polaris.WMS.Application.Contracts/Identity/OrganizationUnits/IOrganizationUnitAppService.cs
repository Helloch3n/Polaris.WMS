using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Polaris.WMS.Identity.OrganizationUnits.Dtos;
using Volo.Abp.Application.Services;

namespace Polaris.WMS.Identity.OrganizationUnits
{
    public interface IOrganizationUnitAppService : IApplicationService
    {
        /// <summary>
        /// 获取组织单元树（用于前端构建树形选择）。
        /// </summary>
        Task<List<OrganizationUnitDto>> GetTreeAsync();

        /// <summary>
        /// 创建组织单元。
        /// </summary>
        Task<OrganizationUnitDto> CreateAsync(CreateOrganizationUnitDto input);

        /// <summary>
        /// 更新组织单元显示名称。
        /// </summary>
        Task<OrganizationUnitDto> UpdateAsync(Guid id, UpdateOrganizationUnitDto input);

        /// <summary>
        /// 删除组织单元。
        /// </summary>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// 将指定用户添加到组织单元（批量）。
        /// </summary>
        Task AddUsersAsync(Guid id, AddUsersToOrganizationUnitDto input);

        /// <summary>
        /// 获取指定组织单元的用户列表（返回用户简要信息）。
        /// </summary>
        Task<List<OrganizationUnitUserDto>> GetUsersAsync(Guid organizationUnitId);
    }
}

