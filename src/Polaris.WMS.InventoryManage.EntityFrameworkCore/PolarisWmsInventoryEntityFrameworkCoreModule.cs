using Microsoft.Extensions.DependencyInjection;
using Polaris.WMS.InventoryManage.Domain.inventories;
using Polaris.WMS.InventoryManage.Domain.Reels;
using Polaris.WMS.InventoryManage.EntityFrameworkCore.Inventories;
using Volo.Abp.Modularity;

namespace Polaris.WMS.InventoryManage.EntityFrameworkCore;

public class PolarisWmsInventoryEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<InventoryDbContext>(options =>
        {
            options.AddDefaultRepositories(includeAllEntities: true);

            // 注册库存模块的自定义仓储
            options.AddRepository<Inventory, InventoryRepository>();
            //options.AddRepository<Reel, ReelRepository>();
        });
    }
}