using Volo.Abp.Application;
using Volo.Abp.Modularity;

namespace Polaris.WMS.Outbound.Application.Contracts;

[DependsOn(
    typeof(AbpDddApplicationContractsModule),
    typeof(WMSDomainSharedModule) // 依赖共享层
)]
public class PolarisWmsOutboundApplicationContractsModule : AbpModule
{
}