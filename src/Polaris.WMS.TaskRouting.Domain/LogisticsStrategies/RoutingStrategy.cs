using Polaris.WMS.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Polaris.WMS.TaskRouting.Domain.LogisticsStrategies;

/// <summary>
/// 路由策略实体（RoutingStrategy）
/// 用于定义仓储内基于条件的移动任务路由规则，例如：当满足某些匹配条件时，
/// 将产生指向指定目标库区（TargetZone）的移动任务。
/// 该聚合根记录了规则名称、优先级、任务类型以及是否启用等元数据。
/// </summary>
public class RoutingStrategy : FullAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// 规则名称，便于在管理界面进行识别。如："高优先级易燃品上架规则"。
    /// </summary>
    public string RuleName { get; private set; } = null!;

    /// <summary>
    /// 规则优先级，数字越小优先级越高（或按项目约定）。用于在多个规则匹配时决定先后顺序。
    /// </summary>
    public int Priority { get; private set; }

    /// <summary>
    /// 该路由策略适用的移动任务类型（例如：上架/拣货/移位等）。
    /// </summary>
    public MoveTaskType TaskType { get; private set; }

    /// <summary>
    /// 是否启用该策略。禁用的策略不会被路由引擎采纳。
    /// </summary>
    public bool IsActive { get; private set; }

    // 匹配条件
    /// <summary>
    /// 源库区 Id（可为空）。当指定时，只有来源为该库区的库存/任务才会匹配此策略。
    /// </summary>
    public Guid? SourceZoneId { get; private set; }

    /// <summary>
    /// 物料类别 Id（可为空）。用于按物料类别进行匹配。
    /// </summary>
    public Guid? ProductCategoryId { get; private set; }

    /// <summary>
    /// 物料 Id（可为空）。当指定时，优先按具体物料进行匹配。
    /// </summary>
    public Guid? ProductId { get; private set; }

    // 路由结果
    /// <summary>
    /// 匹配后目标库区 Id（必填）。表示应将任务路由到的目的库区/货位所属的区。
    /// </summary>
    public Guid TargetZoneId { get; private set; }

    /// <summary>
    /// ORM 使用的受保护无参构造函数。保留以供 Entity Framework 等 ORM 反射创建实体。
    /// 业务代码请通过工厂方法或领域服务创建并初始化完整属性。
    /// </summary>
    protected RoutingStrategy()
    {
    }

    internal RoutingStrategy(string ruleName, int priority, MoveTaskType taskType, bool isActive,
        Guid? productCategoryId, Guid? productId, Guid sourceZoneId, Guid targetZoneId)
    {
        RuleName = ruleName;
        Priority = priority;
        TaskType = taskType;
        IsActive = isActive;
        SourceZoneId = sourceZoneId;
        ProductCategoryId = productCategoryId;
        ProductId = productId;
        TargetZoneId = targetZoneId;
    }
}