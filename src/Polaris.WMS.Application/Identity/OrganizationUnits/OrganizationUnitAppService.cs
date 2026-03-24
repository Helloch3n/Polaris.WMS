using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Polaris.WMS.Identity.OrganizationUnits.Dtos;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace Polaris.WMS.Identity.OrganizationUnits
{
    /// <summary>
    /// 组织单元相关应用服务。
    /// 按 ABP 约定，这个 AppService 会被自动生成到 `/api/app/organization-unit` 路径下。
    /// 方法命名（Get/Create/Update/Delete 等）会被映射为相应的 HTTP 动作。
    /// </summary>
    public class OrganizationUnitAppService : WMSAppService, IOrganizationUnitAppService
    {
        private readonly OrganizationUnitManager _organizationUnitManager;
        private readonly IdentityUserManager _identityUserManager;
        private readonly IRepository<OrganizationUnit, Guid> _organizationUnitRepository;

        public OrganizationUnitAppService(
            OrganizationUnitManager organizationUnitManager,
            IdentityUserManager identityUserManager,
            IRepository<OrganizationUnit, Guid> organizationUnitRepository)
        {
            _organizationUnitManager = organizationUnitManager;
            _identityUserManager = identityUserManager;
            _organizationUnitRepository = organizationUnitRepository;
        }

        /// <summary>
        /// 获取组织单元树（完整列表，按 Code 排序）。
        /// 对应路由：GET /api/app/organization-unit/get-tree
        /// </summary>
        public async Task<List<OrganizationUnitDto>> GetTreeAsync()
        {
            var list = await _organizationUnitRepository.GetListAsync();

            return list
                .OrderBy(x => x.Code)
                .Select(x => new OrganizationUnitDto
                {
                    Id = x.Id,
                    ParentId = x.ParentId,
                    Code = x.Code,
                    DisplayName = x.DisplayName
                })
                .ToList();
        }

        /// <summary>
        /// 创建组织单元（根据父节点生成 Code）。
        /// 对应路由：POST /api/app/organization-unit
        /// </summary>
        public async Task<OrganizationUnitDto> CreateAsync(CreateOrganizationUnitDto input)
        {
            var ou = new OrganizationUnit(
                GuidGenerator.Create(),
                input.DisplayName,
                input.ParentId);

            await _organizationUnitManager.CreateAsync(ou);
            await _organizationUnitRepository.InsertAsync(ou);

            return new OrganizationUnitDto
            {
                Id = ou.Id,
                ParentId = ou.ParentId,
                Code = ou.Code,
                DisplayName = ou.DisplayName
            };
        }

        /// <summary>
        /// 更新组织单元显示名称。
        /// 对应路由：PUT /api/app/organization-unit/{id}
        /// </summary>
        public async Task<OrganizationUnitDto> UpdateAsync(Guid id, UpdateOrganizationUnitDto input)
        {
            var ou = await _organizationUnitRepository.GetAsync(id);

            ou.DisplayName = input.DisplayName;
            await _organizationUnitManager.UpdateAsync(ou);
            await _organizationUnitRepository.UpdateAsync(ou);

            return new OrganizationUnitDto
            {
                Id = ou.Id,
                ParentId = ou.ParentId,
                Code = ou.Code,
                DisplayName = ou.DisplayName
            };
        }

        /// <summary>
        /// 删除组织单元。
        /// 对应路由：DELETE /api/app/organization-unit/{id}
        /// </summary>
        public async Task DeleteAsync(Guid id)
        {
            await _organizationUnitManager.DeleteAsync(id);
        }

        /// <summary>
        /// 将指定用户添加到组织单元（批量）。
        /// 对应路由：POST /api/app/organization-unit/{id}/add-users
        /// 接收 body: { userIds: [ ... ] }
        /// </summary>
        public async Task AddUsersAsync(Guid id, AddUsersToOrganizationUnitDto input)
        {
            await _organizationUnitRepository.GetAsync(id);

            var missing = new List<Guid>();

            foreach (var userId in input.UserIds ?? Enumerable.Empty<Guid>())
            {
                var user = await _identityUserManager.GetByIdAsync(userId);
                if (user == null)
                {
                    missing.Add(userId);
                    continue;
                }

                await _identityUserManager.AddToOrganizationUnitAsync(userId, id);
            }

            if (missing.Any())
            {
                throw new BusinessException("Identity:SomeUsersNotFound")
                    .WithData("MissingUserIds", missing);
            }
        }

        /// <summary>
        /// 获取指定组织单元的成员列表（返回用户简要信息）。
        /// 对应路由（由 ABP 自动映射）：GET /api/app/organization-unit/get-users?organizationUnitId={id}
        /// 如果你需要更友好的 REST 路径 (e.g. /{id}/users)，可以创建专门的 Controller 或使用自定义路由。
        /// </summary>
        public async Task<List<OrganizationUnitUserDto>> GetUsersAsync(Guid organizationUnitId)
        {
            // 确认组织单元存在
            var ou = await _organizationUnitRepository.GetAsync(organizationUnitId);

            // 使用 IdentityUserManager 提供的方法获取组织单元内的用户
            var users = await _identityUserManager.GetUsersInOrganizationUnitAsync(ou);

            return users.Select(u => new OrganizationUnitUserDto
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                Name = u.Name
            }).ToList();
        }
    }
}

