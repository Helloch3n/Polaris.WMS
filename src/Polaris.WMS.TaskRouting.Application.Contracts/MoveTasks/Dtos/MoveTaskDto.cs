using System.ComponentModel.DataAnnotations;
using Polaris.WMS.Tasks;
using Volo.Abp.Application.Dtos;

namespace Polaris.WMS.TaskRouting.Application.Contracts.MoveTasks.Dtos;

public class MoveTaskDto : AuditedEntityDto<Guid>
{
    public Guid Id { get; set; }
    public Guid TaskNo { get; set; }
    public Guid? ContainerId { get; set; } // 如果任务关联了盘具容器
    public string ContainerCode { get; set; }
    public MoveTaskType TaskType { get; set; }
    public MoveTaskStatus Status { get; set; }
    public Guid SourceLocationId { get; set; }
    public string SourceLocationCode { get; set; }
    public Guid TargetLocationId { get; set; }
    public string TargetLocationCode { get; set; }
    public DateTime CreationTime { get; set; }
}