using Polaris.WMS.Tasks;
using Volo.Abp.Application.Dtos;

namespace Polaris.WMS.TaskRouting.Application.Contracts.LogisticsStrategies.Dtos;

/// <summary>
/// 路由策略输出 DTO（包含审计字段）
/// </summary>
public class RoutingStrategyDto : AuditedEntityDto<Guid>
{
    public string RuleName { get; set; } = null!;
    public int Priority { get; set; }
    public MoveTaskType TaskType { get; set; }
    public bool IsActive { get; set; }
    public Guid? SourceZoneId { get; set; }
    public Guid? ProductCategoryId { get; set; }
    public Guid? ProductId { get; set; }
    public Guid TargetZoneId { get; set; }
}