using Microsoft.Extensions.DependencyInjection;
using Polaris.WMS.MasterData.Domain.AccountAliases;
using Polaris.WMS.MasterData.Domain.CostCenters;
using Polaris.WMS.MasterData.Domain.Locations;
using Polaris.WMS.MasterData.Domain.Zones;
using Polaris.WMS.MasterData.EntityFrameworkCore.AccountAliases;
using Polaris.WMS.MasterData.EntityFrameworkCore.CostCenters;
using Polaris.WMS.MasterData.EntityFrameworkCore.Locations;
using Polaris.WMS.MasterData.EntityFrameworkCore.Zones;
using Volo.Abp.Modularity;

namespace Polaris.WMS.MasterData.EntityFrameworkCore;

public class PolarisWmsMasterDataEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<MasterDataDbContext>(options =>
        {
            options.AddDefaultRepositories(includeAllEntities: true);

            // 在属于自己的上下文里注册属于自己的自定义仓储！
            options
                .AddRepository<Zone, ZoneRepository>();
            options
                .AddRepository<Location, LocationRepository>();
            options
                .AddRepository<CostCenter, CostCenterRepository>();
            options
                .AddRepository<AccountAlias, AccountAliasRepository>();
        });
    }
}