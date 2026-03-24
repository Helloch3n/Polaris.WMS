using Microsoft.Extensions.DependencyInjection;
using System;
using Polaris.WMS.Inbound.EntityFrameworkCore;
using Polaris.WMS.InventoryManage.Domain.inventories;
using Polaris.WMS.InventoryManage.Domain.Reels;
using Polaris.WMS.InventoryManage.EntityFrameworkCore;
using Polaris.WMS.InventoryManage.EntityFrameworkCore.Inventories;
using Polaris.WMS.MasterData.Domain.Locations;
using Polaris.WMS.MasterData.Domain.Zones;
using Polaris.WMS.MasterData.EntityFrameworkCore;
using Polaris.WMS.MasterData.EntityFrameworkCore.Locations;
using Polaris.WMS.MasterData.EntityFrameworkCore.Zones;
using Polaris.WMS.Outbound.EntityFrameworkCore;
using Polaris.WMS.TaskRouting.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.PostgreSql;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.Studio;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using Volo.Abp.Uow;

namespace Polaris.WMS.EntityFrameworkCore;

[DependsOn(
    typeof(WMSDomainModule),
    typeof(AbpPermissionManagementEntityFrameworkCoreModule),
    typeof(AbpSettingManagementEntityFrameworkCoreModule),
    typeof(AbpEntityFrameworkCorePostgreSqlModule),
    typeof(AbpBackgroundJobsEntityFrameworkCoreModule),
    typeof(AbpAuditLoggingEntityFrameworkCoreModule),
    typeof(AbpFeatureManagementEntityFrameworkCoreModule),
    typeof(AbpIdentityEntityFrameworkCoreModule),
    typeof(AbpOpenIddictEntityFrameworkCoreModule),
    typeof(AbpTenantManagementEntityFrameworkCoreModule),
    typeof(BlobStoringDatabaseEntityFrameworkCoreModule),
    typeof(AbpCachingStackExchangeRedisModule)
)]
public class WMSEntityFrameworkCoreModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        // https://www.npgsql.org/efcore/release-notes/6.0.html#opting-out-of-the-new-timestamp-mapping-logic
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        WMSEfCoreEntityExtensionMappings.Configure();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<WMSDbContext>(options =>
        {
            /* Remove "includeAllEntities: true" to create
             * default repositories only for aggregate roots */
            options.AddDefaultRepositories(includeAllEntities: true);
            options.ReplaceDbContext<IMasterDataDbContext>();
            options.ReplaceDbContext<ITaskDbContext>();
            options.ReplaceDbContext<IInventoryDbContext>();
            options.ReplaceDbContext<IInboundDbContext>();
            options.ReplaceDbContext<IOutBoundDbContext>();

            // 如果模块中有自定义仓储(如 IInventoryRepository)，使用 ABP 的标准语法注册给 WMSDbContext
            // 主数据模块的自定义仓储
            //options.AddRepository<Zone, ZoneRepository>();
            //options.AddRepository<Location, LocationRepository>();

            // 库存模块的自定义仓储
            //options.AddRepository<Inventory, InventoryRepository>();
            //options.AddRepository<Reel, ReelRepository>();
        });

        // // 显式绑定自定义仓储接口，避免模块化改造后接口未被自动暴露
        // // 1. 主数据模块 (MasterData) 的自定义仓储
        context.Services.AddTransient<IZoneRepository, ZoneRepository>();
        context.Services.AddTransient<ILocationRepository, LocationRepository>();
        //
        // // 2. 库存模块 (InventoryManage) 的自定义仓储
        context.Services.AddTransient<IInventoryRepository, InventoryRepository>();
        // //context.Services.AddTransient<IReelRepository, ReelRepository>();

        if (AbpStudioAnalyzeHelper.IsInAnalyzeMode)
        {
            return;
        }

        Configure<AbpDbContextOptions>(options =>
        {
            /* The main point to change your DBMS.
             * See also WMSDbContextFactory for EF Core tooling. */

            options.UseNpgsql();
        });
    }
}