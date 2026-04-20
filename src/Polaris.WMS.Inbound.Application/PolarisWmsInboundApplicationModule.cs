using Polaris.WMS.Inbound.Application.Contracts;
using Polaris.WMS.Inbound.Domain;
using Polaris.WMS.InventoryManage.Application.Contracts;
using Polaris.WMS.MasterData.Application.Contracts;
using Volo.Abp.Application;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;

namespace Polaris.WMS.Inbound.Application;

[DependsOn(
    typeof(AbpDddApplicationModule), // 1. 依赖 ABP 核心应用层基础包
    typeof(PolarisWmsInboundDomainModule), // 2. 依赖本模块的 Domain 层
    typeof(AbpMapperlyModule), // 3. 依赖映射工具包
    typeof(WMSApplicationContractsModule), // (可选) 如果你的 DTO 全写在全局 Contracts 里，就加上这个
    typeof(PolarisWmsInboundApplicationContractsModule),
    typeof(PolarisWmsInventoryApplicationContractsModule),
    typeof(PolarisWmsMasterDataApplicationContractsModule)
)]
public class PolarisWmsInboundApplicationModule: AbpModule
{
}