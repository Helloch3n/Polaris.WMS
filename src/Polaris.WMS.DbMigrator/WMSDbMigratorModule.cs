using Polaris.WMS.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Polaris.WMS.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(WMSEntityFrameworkCoreModule),
    typeof(WMSApplicationContractsModule)
)]
public class WMSDbMigratorModule : AbpModule
{
}

