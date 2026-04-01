using Polaris.WMS.TaskRouting.Application.Contracts.MoveTasks.Dtos;
using Polaris.WMS.TaskRouting.Domain.MoveTasks;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace Polaris.WMS.TaskRouting.Application.MoveTasks;

public partial class MoveTaskMappers
{
    [Mapper]
    public partial class ProductMappers : MapperBase<MoveTask, MoveTaskDto>
    {
        public override partial MoveTaskDto Map(MoveTask source);
        public override partial void Map(MoveTask source, MoveTaskDto destination);
    }
}