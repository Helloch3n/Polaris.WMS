using Polaris.WMS.PurchaseOrders;
using Volo.Abp;
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

    public IReadOnlyCollection<PurchaseOrderDetail> Details => _details;

    /// <summary>
    /// EF Core 使用的受保护构造函数，保留用于 ORM 反射创建实体。
    /// 领域代码应通过工厂方法或领域行为创建聚合实例。
    /// </summary>
    protected PurchaseOrder()
    {
    }
    
    /// <summary>
    /// 构造函数：创建时必须具备核心主数据
    /// </summary>
    public PurchaseOrder(Guid id, string poNo, Guid supplierId, string supplierCode, string supplierName, DateTime orderDate, DateTime? expectedDeliveryDate = null)
        : base(id)
    {
        Check.NotNullOrWhiteSpace(poNo, nameof(poNo));
        Check.NotNullOrWhiteSpace(supplierCode, nameof(supplierCode));

        PoNo = poNo;
        SupplierId = supplierId;
        SupplierCode = supplierCode;
        SupplierName = supplierName;
        OrderDate = orderDate;
        ExpectedDeliveryDate = expectedDeliveryDate;
        Status = PurchaseOrderStatus.Open; // 初始化为开放状态
    }
    
    /// <summary>
    /// 通过聚合根添加明细，保护聚合内的一致性
    /// </summary>
    public PurchaseOrderDetail AddDetail(Guid detailId, int lineNo, Guid productId, string productCode, string productName, string uom, decimal expectedQty, bool isQualityCheckRequired)
    {
        if (_details.Any(x => x.LineNo == lineNo))
        {
            throw new UserFriendlyException($"采购单 {PoNo} 已存在行号为 {lineNo} 的明细！");
        }

        var detail = new PurchaseOrderDetail(detailId, Id, lineNo, productId, productCode, productName, uom, expectedQty, isQualityCheckRequired);
        _details.Add(detail);
        return detail;
    }
    
    /// <summary>
    /// 状态流转：根据明细的收货情况，推演 PO 的总体状态。
    /// </summary>
    public void RefreshStatus()
    {
        if (Status == PurchaseOrderStatus.Closed) 
            return; // 强制关闭的单据不再自动流转

        if (!_details.Any()) return;

        bool isAllCompleted = _details.All(x => x.ReceivedQty >= x.ExpectedQty);
        bool isAnyReceived = _details.Any(x => x.ReceivedQty > 0);

        if (isAllCompleted)
        {
            Status = PurchaseOrderStatus.Completed;
        }
        else if (isAnyReceived)
        {
            Status = PurchaseOrderStatus.PartialReceived;
        }
    }
    
    /// <summary>
    /// 业务行为：手工强制结案
    /// </summary>
    public void ForceClose()
    {
        Status = PurchaseOrderStatus.Closed;
    }
    
    /// <summary>
    /// 更新主表基础信息
    /// </summary>
    public void UpdateBasicInfo(DateTime orderDate, DateTime? expectedDeliveryDate)
    {
        OrderDate = orderDate;
        ExpectedDeliveryDate = expectedDeliveryDate;
    }

    /// <summary>
    /// 通过聚合根更新明细的期望数量
    /// </summary>
    public void UpdateDetailExpectedQty(int lineNo, decimal newExpectedQty)
    {
        var detail = _details.FirstOrDefault(x => x.LineNo == lineNo);
        if (detail == null)
        {
            throw new UserFriendlyException($"采购单 {PoNo} 中不存在行号为 {lineNo} 的明细！");
        }

        // 调用明细内部的更新方法
        detail.UpdateExpectedQty(newExpectedQty);

        // 数量变化后，可能会导致单据状态发生变化（比如从 PartialReceived 变成了 Completed）
        RefreshStatus();
    }
}