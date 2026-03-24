using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace Polaris.WMS.InventoryManage.Domain;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(WMSDomainSharedModule) // 依赖共享层
)]
public class PolarisWmsInventoryDomainModule : AbpModule
{
}