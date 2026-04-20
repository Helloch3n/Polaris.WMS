using Polaris.WMS.Asns;
using Volo.Abp;
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
    
    public AdvancedShippingNotice(Guid id, string asnNo, Guid supplierId, string supplierCode, string supplierName, DateTime? expectedArrivalTime = null)
        : base(id)
    {
        Check.NotNullOrWhiteSpace(asnNo, nameof(asnNo));
        AsnNo = asnNo;
        SupplierId = supplierId;
        SupplierCode = supplierCode;
        SupplierName = supplierName;
        ExpectedArrivalTime = expectedArrivalTime;
        Status = AsnStatus.Pending;
    }
    
    public AsnDetail AddDetail(Guid id, string scmAsnRowNo, Guid sourcePoId,string sourcePoNo, int sourcePoLineNo, Guid productId, string productCode, string productName, string uom, decimal expectedQty, string supplierBatchNo = null, string licensePlate = null)
    {
        var detail = new AsnDetail(id, Id, scmAsnRowNo, sourcePoId,sourcePoNo, sourcePoLineNo, productId, productCode, productName, uom, expectedQty, supplierBatchNo, licensePlate);
        _details.Add(detail);
        return detail;
    }

    /// <summary>
    /// 状态机推演
    /// </summary>
    public void RefreshStatus()
    {
        if (!_details.Any()) return;

        bool isAllCompleted = _details.All(x => x.ReceivedQty >= x.ExpectedQty);
        bool isAnyReceived = _details.Any(x => x.ReceivedQty > 0);

        if (isAllCompleted) Status = AsnStatus.Completed;
        else if (isAnyReceived) Status = AsnStatus.Receiving;
    }

    public void AddReceivedQty(Guid asnDetailId, decimal qty)
    {
        var detail = _details.FirstOrDefault(x => x.Id == asnDetailId);
        if (detail == null)
        {
            throw new UserFriendlyException($"ASN 单据 {AsnNo} 中不存在明细 {asnDetailId}！");
        }

        detail.AddReceivedQty(qty);
        RefreshStatus();
    }
    
    /// <summary>
    /// 充血模型：更新主表基础信息
    /// </summary>
    public void UpdateBasicInfo(DateTime? expectedArrivalTime)
    {
        ExpectedArrivalTime = expectedArrivalTime;
    }

    /// <summary>
    /// 充血模型：通过聚合根更新明细
    /// </summary>
    public void UpdateDetailInfo(string scmAsnRowNo, decimal expectedQty, string supplierBatchNo, string licensePlate)
    {
        var detail = _details.FirstOrDefault(x => x.ScmAsnRowNo == scmAsnRowNo);
        if (detail == null)
        {
            throw new UserFriendlyException($"ASN 单据 {AsnNo} 中不存在行号为 {scmAsnRowNo} 的明细！");
        }

        detail.UpdateInfo(expectedQty, supplierBatchNo, licensePlate);
        RefreshStatus();
    }

    /// <summary>
    /// 充血模型：安全移除 SCM 端已经删除的明细行
    /// </summary>
    public void RemoveDetailsNotIn(IEnumerable<string> incomingScmRowNos)
    {
        // 找出本地有，但 SCM 传过来的报文里没有的行
        var detailsToRemove = _details.Where(x => !incomingScmRowNos.Contains(x.ScmAsnRowNo)).ToList();

        foreach (var detail in detailsToRemove)
        {
            // 极限防呆：哪怕主状态有 Bug 没拦住，只要这行已经收过货，坚决不准删！
            if (detail.ReceivedQty > 0)
            {
                throw new UserFriendlyException($"领域规则拦截：无法删除行号 {detail.ScmAsnRowNo}，因为该行已实收 {detail.ReceivedQty}，不允许被移除！");
            }
            _details.Remove(detail);
        }
        
        RefreshStatus();
    }
}