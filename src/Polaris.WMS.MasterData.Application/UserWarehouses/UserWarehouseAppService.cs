using Polaris.WMS.MasterData.Application.Contracts.Users.Dtos;
using Polaris.WMS.MasterData.Domain.Integration.Identity.Users;
using Polaris.WMS.MasterData.Domain.warehouses;
using Polaris.WMS.Users;
using Polaris.WMS.Users.Dtos;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Polaris.WMS.MasterData.Application.UserWarehouses
{
    /// <summary>
    /// 用户-仓库 关联管理应用服务。
    /// </summary>
    public class UserWarehouseAppService(
        IRepository<UserWarehouse, Guid> userWarehouseRepository,
        IRepository<Warehouse, Guid> warehouseRepository,
        IUserAllocationAdapter locationAdapter)
        : ApplicationService, IUserWarehouseAppService
    {
        /// <summary>
        /// 将多个用户加入指定仓库（批量）。
        /// </summary>
        public async Task AddUsersAsync(Guid warehouseId, AddUsersToWarehouseDto input)
        {
            await warehouseRepository.GetAsync(warehouseId);

            var missing = new List<Guid>();

            foreach (var userId in input.UserIds ?? Enumerable.Empty<Guid>())
            {
                // var user = await userRepository.FirstOrDefaultAsync(u => u.Id == userId);
                // if (user == null)
                // {
                //     missing.Add(userId);
                //     continue;
                // }

                var exists =
                    await userWarehouseRepository.FirstOrDefaultAsync(x =>
                        x.UserId == userId && x.WarehouseId == warehouseId);
                if (exists == null)
                {
                    await userWarehouseRepository.InsertAsync(new UserWarehouse(GuidGenerator.Create(), userId,
                        warehouseId));
                }
            }

            if (missing.Any())
            {
                throw new BusinessException("Identity:SomeUsersNotFound").WithData("MissingUserIds", missing);
            }
        }

        /// <summary>
        /// 将多个用户从指定仓库移除（批量）。
        /// </summary>
        public async Task RemoveUsersAsync(Guid warehouseId, AddUsersToWarehouseDto input)
        {
            var mappings = await userWarehouseRepository.GetListAsync(x =>
                x.WarehouseId == warehouseId && input.UserIds.Contains(x.UserId));
            foreach (var m in mappings)
            {
                await userWarehouseRepository.DeleteAsync(m);
            }
        }

        /// <summary>
        /// 获取指定仓库下的用户列表（简要信息）。
        /// </summary>
        public async Task<List<WarehouseUserDto>> GetUsersByWarehouseAsync(Guid warehouseId)
        {
            await warehouseRepository.GetAsync(warehouseId);

            var mappings = await userWarehouseRepository.GetListAsync(x => x.WarehouseId == warehouseId);
            var userIds = mappings.Select(x => x.UserId).Distinct().ToList();

            if (!userIds.Any()) return new List<WarehouseUserDto>();

            var users = await locationAdapter.GetUserInfoAsync(userIds);
            //var userQuery = await userRepository.GetQueryableAsync();
            //var users = await AsyncExecuter.ToListAsync(userQuery.Where(u => userIds.Contains(u.Id)));

            return users.Select(u => new WarehouseUserDto
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                Name = u.Name
            }).ToList();
        }

        /// <summary>
        /// 获取指定用户所属的仓库列表（仓库 Id 列表）。
        /// </summary>
        public async Task<List<Guid>> GetWarehousesByUserAsync(Guid userId)
        {
            var mappings = await userWarehouseRepository.GetListAsync(x => x.UserId == userId);
            return mappings.Select(x => x.WarehouseId).Distinct().ToList();
        }
    }
}