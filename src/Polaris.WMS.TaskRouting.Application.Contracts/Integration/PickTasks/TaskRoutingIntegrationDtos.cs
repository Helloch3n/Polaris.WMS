using Polaris.WMS.Tasks;

namespace Polaris.WMS.TaskRouting.Application.Contracts.Integration.PickTasks;

public class CreatePickDownTaskInput
{
    public Guid ContainerId { get; set; }

    public string ContainerCode { get; set; } = string.Empty;

    public Guid SourceLocationId { get; set; }

    public string SourceLocationCode { get; set; } = string.Empty;

    public Guid TargetLocationId { get; set; }

    public string TargetLocationCode { get; set; } = string.Empty;

    public MoveTaskType TaskType { get; set; } = MoveTaskType.PickDown;
}

public class PickTaskIntegrationDto
{
    public Guid TaskId { get; set; }

    public string TaskNo { get; set; } = string.Empty;
}

