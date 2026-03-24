using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace Polaris.WMS.Outbound.Domain;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(WMSDomainSharedModule) // 依赖共享层
)]
public class PolarisWmsOutboundDomainModule : AbpModule
{
}