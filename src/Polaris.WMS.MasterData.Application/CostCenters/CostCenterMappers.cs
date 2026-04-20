using Polaris.WMS.MasterData.Application.Contracts.CostCenters.Dtos;
using Polaris.WMS.MasterData.Domain.CostCenters;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace Polaris.WMS.MasterData.Application.CostCenters;

[Mapper]
public partial class CostCenterMappers : MapperBase<CostCenter, CostCenterDto>
{
    public override partial CostCenterDto Map(CostCenter source);
    public override partial void Map(CostCenter source, CostCenterDto destination);
}


