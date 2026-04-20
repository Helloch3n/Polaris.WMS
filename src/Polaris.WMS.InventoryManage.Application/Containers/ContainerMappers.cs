using Polaris.WMS.InventoryManage.Application.Contracts.Containers.Dtos;
using Polaris.WMS.InventoryManage.Domain.Containers;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace Polaris.WMS.InventoryManage.Application.Containers;

[Mapper]
public partial class ReelMappers : MapperBase<Container, ContainerDto>
{
    [MapProperty(nameof(Container.IsLocked), nameof(ContainerDto.IsLocked))]
    public override partial ContainerDto Map(Container source);

    public override partial void Map(Container source, ContainerDto destination);

    public partial void Map(CreateUpdateContainerDto source, Container destination);
}
