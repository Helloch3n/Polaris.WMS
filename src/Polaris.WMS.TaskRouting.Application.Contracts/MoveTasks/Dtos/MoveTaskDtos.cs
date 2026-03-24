using System.ComponentModel.DataAnnotations;
using Polaris.WMS.Tasks;

namespace Polaris.WMS.TaskRouting.Application.Contracts.MoveTasks.Dtos;

public class CreateMoveTaskDto
{
    [Required] public Guid ContainerId { get; set; }

    public Guid? FromLocationId { get; set; }

    [Required] public Guid TargetLocationId { get; set; }

    public MoveTaskType TaskType { get; set; }
}

public class CompleteMoveTaskDto
{
    [Required] public Guid TaskId { get; set; }

    /// <summary>
    /// 叉车工实际扫描的墙上大区标牌 ID，或具体的货架格 ID
    /// </summary>
    [Required]
    public Guid ScannedLocationId { get; set; }
}