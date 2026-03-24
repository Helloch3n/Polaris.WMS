using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace Polaris.WMS.TaskRouting.Domain;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(WMSDomainSharedModule) // 依赖共享层
)]
public class PolarisWmsTaskDomainModule : AbpModule
{
}