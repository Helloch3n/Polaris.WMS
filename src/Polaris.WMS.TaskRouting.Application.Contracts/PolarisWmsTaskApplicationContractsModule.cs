using Volo.Abp.Application;
using Volo.Abp.Modularity;

namespace Polaris.WMS.TaskRouting.Application.Contracts;

[DependsOn(
    typeof(AbpDddApplicationContractsModule),
    typeof(WMSDomainSharedModule) // 依赖共享层
)]
public class PolarisWmsTaskApplicationContractsModule : AbpModule
{
}