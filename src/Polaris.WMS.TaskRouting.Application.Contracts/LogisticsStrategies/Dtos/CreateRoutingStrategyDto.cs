using System.ComponentModel.DataAnnotations;
using Polaris.WMS.Tasks;

namespace Polaris.WMS.TaskRouting.Application.Contracts.LogisticsStrategies.Dtos;

/// <summary>
/// 创建路由策略的输入 DTO。
/// </summary>
public class CreateRoutingStrategyDto
{
    /// <summary>
    /// 规则名称
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string RuleName { get; set; } = null!;

    /// <summary>
    /// 规则优先级，数字越小优先级越高
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// 适用的移动任务类型
    /// </summary>
    public MoveTaskType TaskType { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 源库区 Id（可空）
    /// </summary>
    public Guid SourceZoneId { get; set; }

    /// <summary>
    /// 物料类别 Id（可空）
    /// </summary>
    public Guid? ProductCategoryId { get; set; }

    /// <summary>
    /// 物料 Id（可空）
    /// </summary>
    public Guid? ProductId { get; set; }

    /// <summary>
    /// 目标库区 Id（必填）
    /// </summary>
    [Required]
    public Guid TargetZoneId { get; set; }
}