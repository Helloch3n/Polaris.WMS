using Polaris.WMS.Asns;
using Volo.Abp.Domain.Entities.Auditing;

namespace Polaris.WMS.Inbound.Domain.Asns;

/// <summary>
/// 预先送货通知（ASN）聚合根。
/// 表示供应商发送到仓库的送货通知单，包含送货车辆、供应商信息、预计到达时间及明细集合等。
/// </summary>
public class AdvancedShippingNotice : FullAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// ASN 单号，例如：ASN-2026-088。
    /// </summary>
    public string AsnNo { get; private set; } // ASN单号 (如: ASN-2026-088)

    public Guid SupplierId { get; private set; }

    /// <summary>
    /// 供应商编码（用于展示与快速定位）。
    /// </summary>
    public string SupplierCode { get; private set; }

    /// <summary>
    /// 供应商名称（冗余字段，用于展示与历史快照）。
    /// </summary>
    public string SupplierName { get; private set; }

   

    /// <summary>
    /// 单据状态。
    /// 可能的值示例：Pending（待收货）、Receiving（收货中）、Completed（完成）。
    /// </summary>
    public AsnStatus Status { get; private set; } // 状态: Pending(待收货), Receiving(收货中), Completed(完成)

    /// <summary>
    /// 预计到货时间（可空）。
    /// </summary>
    public DateTime? ExpectedArrivalTime { get; private set; } // 预计到货时间

    /// <summary>
    /// ASN 明细集合（导航属性），表示该 ASN 下的所有明细行。
    /// </summary>
    private readonly List<AsnDetail> _details = new();

    public IReadOnlyCollection<AsnDetail> Details => _details;


    /// <summary>
    /// EF Core 使用的受保护构造函数，仅供 ORM 创建实体实例。
    /// 领域代码应通过工厂或领域行为创建聚合实例。
    /// </summary>
    protected AdvancedShippingNotice()
    {
    }
}