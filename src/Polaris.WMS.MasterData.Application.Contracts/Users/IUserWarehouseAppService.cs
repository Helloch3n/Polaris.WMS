using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Polaris.WMS.MasterData.Application.Contracts.Users.Dtos;
using Polaris.WMS.Users.Dtos;
using Volo.Abp.Application.Services;

namespace Polaris.WMS.Users
{
    public interface IUserWarehouseAppService : IApplicationService
    {
        /// <summary>
        /// 将多个用户加入指定仓库（批量）。
        /// </summary>
        Task AddUsersAsync(Guid warehouseId, AddUsersToWarehouseDto input);

        /// <summary>
        /// 将多个用户从指定仓库移除（批量）。
        /// </summary>
        Task RemoveUsersAsync(Guid warehouseId, AddUsersToWarehouseDto input);

        /// <summary>
        /// 获取指定仓库下的用户列表（简要信息）。
        /// </summary>
        Task<List<WarehouseUserDto>> GetUsersByWarehouseAsync(Guid warehouseId);

        /// <summary>
        /// 获取指定用户所属的仓库列表（仓库 Id 列表）。
        /// </summary>
        Task<List<Guid>> GetWarehousesByUserAsync(Guid userId);
    }
}
