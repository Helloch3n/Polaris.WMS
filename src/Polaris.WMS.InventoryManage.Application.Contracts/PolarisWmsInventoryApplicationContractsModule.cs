using Volo.Abp.Application;
using Volo.Abp.Modularity;

namespace Polaris.WMS.InventoryManage.Application.Contracts;

[DependsOn(
    typeof(AbpDddApplicationContractsModule),
    typeof(WMSDomainSharedModule) // 依赖共享层
)]
public class PolarisWmsInventoryApplicationContractsModule: AbpModule
{
}