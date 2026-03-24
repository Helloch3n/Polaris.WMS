using Volo.Abp.Application;
using Volo.Abp.Modularity;

namespace Polaris.WMS.Inound.Application.Contracts;

[DependsOn(
    typeof(AbpDddApplicationContractsModule),
    typeof(WMSDomainSharedModule) // 依赖共享层
)]
public class PolarisWmsInboundApplicationContractsModule: AbpModule
{
}