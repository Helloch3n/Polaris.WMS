using Volo.Abp.Modularity;

namespace Polaris.WMS;

[DependsOn(
    typeof(WMSApplicationModule),
    typeof(WMSDomainTestModule)
)]
public class WMSApplicationTestModule : AbpModule
{

}

