using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Polaris.WMS.Inbound.Domain.Asns;

/// <summary>
/// ASN（预先送货通知）明细行实体。
/// 表示供应商发运到仓库的一条明细记录，包含源采购单信息、物料与批次、数量等字段。
/// </summary>
public class AsnDetail : AuditedEntity<Guid>
{
    /// <summary>
    /// 所属 ASN 的 Id（关联聚合根）。
    /// </summary>
    public Guid AsnId { get; private set; }

    public string ScmAsnRowNo { get; private set; }

    public Guid SourcePoId { get; private set; }

    /// <summary>
    /// 源头采购单号（ERP 中的采购单编号）。
    /// 用于追溯该批货对应的采购单来源。
    /// </summary>
    public string SourcePoNo { get; private set; }

    /// <summary>
    /// 源头采购单行号（ERP 行号，如 10、20）。
    /// 用于精确定位对应的采购单明细行。
    /// </summary>
    public int SourcePoLineNo { get; private set; }

    /// <summary>
    /// 物料主键 Id（引用商品/物料上下文）。
    /// </summary>
    public Guid ProductId { get; private set; }

    /// <summary>
    /// 物料编码（用于展示与快速定位）。
    /// </summary>
    public string ProductCode { get; private set; }

    /// <summary>
    /// 物料名称（冗余字段，便于展示与历史快照）。
    /// </summary>
    public string ProductName { get; private set; }

    /// <summary>
    /// 供应商提供的批次号（供应商原厂批次）。
    /// </summary>
    public string SupplierBatchNo { get; private set; } // 供应商原厂批次号

    /// <summary>
    /// 送货车牌号（可选），用于现场核对与门卫管理。
    /// </summary>
    public string LicensePlate { get; private set; } // 送货车牌号 (选填，方便月台门卫核对)

    /// <summary>
    /// 计量单位（例如：kg、盘、m）。
    /// </summary>
    public string UoM { get; private set; } // 单位 (如 kg, 盘, m)

    /// <summary>
    /// ASN 声明的本次发货数量（期望数量）。
    /// </summary>
    public decimal ExpectedQty { get; private set; } // ASN 声明的本次发货数量

    /// <summary>
    /// 实际扫码收到的数量（用于收货累加与防呆）。
    /// </summary>
    public decimal ReceivedQty { get; private set; } // 实际扫码收到的数量

    protected AsnDetail()
    {
    }

    internal AsnDetail(Guid id, Guid asnId, string scmAsnRowNo, Guid sourcePoId,string sourcePoNo, int sourcePoLineNo, Guid productId,
        string productCode, string productName, string uom, decimal expectedQty, string supplierBatchNo,
        string licensePlate)
        : base(id)
    {
        AsnId = asnId;
        ScmAsnRowNo = scmAsnRowNo;
        SourcePoId = sourcePoId;
        SourcePoNo = sourcePoNo;
        SourcePoLineNo = sourcePoLineNo;
        ProductId = productId;
        ProductCode = productCode;
        ProductName = productName;
        UoM = uom;
        ExpectedQty = expectedQty >= 0 ? expectedQty : throw new ArgumentException("期望数量不能为负数");
        SupplierBatchNo = supplierBatchNo;
        LicensePlate = licensePlate;
        ReceivedQty = 0;
    }

    /// <summary>
    /// 核心领域方法：扫码收货并累加已收数量。
    /// 说明：方法不会在内部进行超收抛错，允许上层策略（应用层或领域策略）决定是否对超收进行限制或警告。
    /// </summary>
    /// <param name="qty">本次扫码收货的数量。</param>
    public void AddReceivedQty(decimal qty)
    {
        ReceivedQty += qty;
    }
    /// <summary>
    /// 充血模型：更新明细信息
    /// 领域层防呆：尽管通常只有 Pending 状态才允许修改，但依然要加上底层保护，防止意外超扣。
    /// </summary>
    internal void UpdateInfo(decimal newExpectedQty, string newSupplierBatchNo, string newLicensePlate)
    {
        if (newExpectedQty < 0) throw new ArgumentException("期望数量不能为负数", nameof(newExpectedQty));
        
        if (newExpectedQty < ReceivedQty)
        {
            throw new UserFriendlyException($"领域规则拦截：批次行 {ScmAsnRowNo} 已实收 {ReceivedQty}{UoM}，期望数量不能修改为 {newExpectedQty}！");
        }

        ExpectedQty = newExpectedQty;
        SupplierBatchNo = newSupplierBatchNo;
        LicensePlate = newLicensePlate;
    }
}