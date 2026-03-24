using Volo.Abp.Modularity;

namespace Polaris.WMS;

[DependsOn(
    typeof(WMSDomainModule),
    typeof(WMSTestBaseModule)
)]
public class WMSDomainTestModule : AbpModule
{

}

