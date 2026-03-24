using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace Polaris.WMS.Integration;

public class UserIntegrationService(IRepository<IdentityUser, Guid> userRepository)
    : ApplicationService, IUserIntegrationService
{
    public async Task<List<UserIntegrationDto>> GetUserProfileAsync(List<Guid> userIds)
    {
        if (userIds == null || !userIds.Any())
        {
            return new List<UserIntegrationDto>();
        }

        // 2. 核心查询：你的写法非常标准
        var userQuery = await userRepository.GetQueryableAsync();
        var users = await AsyncExecuter.ToListAsync(userQuery.Where(u => userIds.Contains(u.Id)));

        // 3. 结果组装：将底层实体映射为集成的 DTO 交出
        return users.Select(u => new UserIntegrationDto
        {
            Id = u.Id,
            UserName = u.UserName,
            Name = u.Name, // IdentityUser 的真实姓名
            Email = u.Email, // 邮箱
            // JobTitle = u.GetProperty<string>("JobTitle") // 如果你们用到了 ABP 的扩展属性(ExtraProperties)存岗位，可以这么取
        }).ToList();
    }
}