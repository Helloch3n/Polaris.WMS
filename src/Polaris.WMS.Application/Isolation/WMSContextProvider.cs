using Microsoft.AspNetCore.Http;
using Polaris.WMS.Permissions;
using System;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Threading;

namespace Polaris.WMS.Isolation
{
    public class WMSContextProvider : IWMSContextProvider, ITransientDependency
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPermissionChecker _permissionChecker;

        public WMSContextProvider(
            IHttpContextAccessor httpContextAccessor,
            IPermissionChecker permissionChecker)
        {
            _httpContextAccessor = httpContextAccessor;
            _permissionChecker = permissionChecker;
        }

        public Guid? CurrentDepartmentId
        {
            get
            {
                var headerValue = _httpContextAccessor.HttpContext?.Request.Headers["X-Current-Department"].ToString();

                // 1. 如果前端明确传了车间 ID，就按传的过滤（即便有特权，选了具体车间也只看具体车间）
                if (!string.IsNullOrEmpty(headerValue) && Guid.TryParse(headerValue, out Guid deptId))
                {
                    return deptId;
                }

                // 2. 如果前端没传值，检查是否拥有“查看所有车间”的上帝权限
                var hasGodMode = AsyncHelper.RunSync(() =>
                    _permissionChecker.IsGrantedAsync(WMSPermissions.Global.ViewAllDepartmentsData));

                if (hasGodMode)
                {
                    return null; // 特权放行，EF Core 将不再追加部门 WHERE 条件
                }

                // 3. 既不传车间 ID，又没有特权，直接拒绝服务
                throw new UnauthorizedAccessException("WMS: 您没有全局车间查看权限，请在右上角选择具体的车间！");
            }
        }

        public Guid? CurrentWarehouseId
        {
            get
            {
                var headerValue = _httpContextAccessor.HttpContext?.Request.Headers["X-Current-Warehouse"].ToString();

                // 1. 如果前端明确传了仓库 ID，直接返回
                if (!string.IsNullOrEmpty(headerValue) && Guid.TryParse(headerValue, out Guid warehouseId))
                {
                    return warehouseId;
                }

                // 2. 检查是否拥有“查看所有仓库”的上帝权限 (这里解开了注释，并使用了正确的常量)
                var hasGodMode = AsyncHelper.RunSync(() =>
                    _permissionChecker.IsGrantedAsync(WMSPermissions.Global.ViewAllWarehousesData));

                if (hasGodMode)
                {
                    return null; // 特权放行，EF Core 将不再追加仓库 WHERE 条件
                }

                // 3. 铁面无私的拦截：不传仓库 ID 且没权限，直接拒绝服务！
                throw new UnauthorizedAccessException("WMS: 您没有全局仓库查看权限，请在右上角选择具体的仓库！");
            }
        }
    }
}