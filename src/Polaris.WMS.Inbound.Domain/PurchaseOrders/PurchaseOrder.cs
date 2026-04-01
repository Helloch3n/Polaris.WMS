using Polaris.WMS.PurchaseOrders;
using Volo.Abp.Domain.Entities.Auditing;

namespace Polaris.WMS.Inbound.Domain.PurchaseOrders;

/// <summary>
/// 采购订单聚合根。
/// 表示一张采购单的聚合根实体，包含基本信息、状态与明细集合。
/// </summary>
public class PurchaseOrder : FullAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// 采购单号，例如：PO-2026-001。
    /// </summary>
    public string PoNo { get; private set; }

    /// <summary>
    /// 供应商主键 Id（关联供应商上下文中的实体）。
    /// </summary>
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
    /// 可能的值：Open（开放）、PartialReceived（部分接收）、Completed（完成）、Closed（强制关闭）。
    /// </summary>
    public PurchaseOrderStatus Status { get; private set; }

    /// <summary>
    /// 订单日期（下单日期）。
    /// </summary>
    public DateTime OrderDate { get; private set; }

    /// <summary>
    /// 预计交货日期（可为空）。
    /// </summary>
    public DateTime? ExpectedDeliveryDate { get; private set; }

    /// <summary>
    /// 采购单明细集合（导航属性）。
    /// 通过聚合内的明细表示每行物料信息与数量。
    /// </summary>
    private readonly List<PurchaseOrderDetail> _details = new();

    public IReadOnlyCollection<PurchaseOrderDetail> Details() => _details;

    /// <summary>
    /// EF Core 使用的受保护构造函数，保留用于 ORM 反射创建实体。
    /// 领域代码应通过工厂方法或领域行为创建聚合实例。
    /// </summary>
    protected PurchaseOrder()
    {
    }
}