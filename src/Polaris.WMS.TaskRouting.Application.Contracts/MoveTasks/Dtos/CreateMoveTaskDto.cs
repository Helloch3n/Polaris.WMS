using System.ComponentModel.DataAnnotations;
using Polaris.WMS.Tasks;

namespace Polaris.WMS.TaskRouting.Application.Contracts.MoveTasks.Dtos;

public class CreateMoveTaskDto
{
    [Required] public Guid ContainerId { get; set; }

    public string? ContainerCode { get; set; }

    public Guid FromLocationId { get; set; }

    [Required] public Guid TargetLocationId { get; set; }

    public MoveTaskType TaskType { get; set; }
}