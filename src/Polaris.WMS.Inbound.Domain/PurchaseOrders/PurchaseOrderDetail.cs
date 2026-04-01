using System.Runtime.CompilerServices;
using Volo.Abp.Domain.Entities.Auditing;

namespace Polaris.WMS.Inbound.Domain.PurchaseOrders;

/// <summary>
/// 采购单明细行实体。
/// 表示采购订单中的一行物料数据，包含物料信息、数量控制与业务防呆属性。
/// </summary>
public class PurchaseOrderDetail : AuditedEntity<Guid>
{
    /// <summary>
    /// 关联的采购单 Id。
    /// </summary>
    public Guid PurchaseOrderId { get; private set; }

    /// <summary>
    /// 行号（ERP 中的行号，例如 10、20）。
    /// </summary>
    public int LineNo { get; private set; } // ERP 中的行号 (如 10, 20)

    public Guid ProductId { get; private set; }

    // 物料信息
    /// <summary>
    /// 物料编码。
    /// </summary>
    public string ProductCode { get; private set; }

    /// <summary>
    /// 物料名称。
    /// </summary>
    public string ProductName { get; private set; }

    /// <summary>
    /// 计量单位（如 kg、盘、m）。
    /// </summary>
    public string UoM { get; private set; } // 单位 (如 kg, 盘, m)

    // 数量控制 (防超收核心)
    /// <summary>
    /// 订单期望数量（采购数量）。
    /// </summary>
    public decimal ExpectedQty { get; private set; } // 订单总数

    /// <summary>
    /// 已接收数量（WMS 月台已接收，用于防止超收）。
    /// </summary>
    public decimal ReceivedQty { get; private set; } // WMS 月台已接收数量 (对接 Oracle Receive)

    /// <summary>
    /// 已交付/上架数量（WMS 已上架或入库的数量）。
    /// </summary>
    public decimal DeliveredQty { get; private set; } // WMS 已上架/入库数量 (对接 Oracle Deliver)

    // 业务防呆属性
    /// <summary>
    /// 是否需要质检（可从物料主数据带出，指导收货后的初始状态）。
    /// </summary>
    public bool IsQualityCheckRequired { get; private set; } // 是否需要质检 (可从物料主数据带出，指导收货后的初始状态)

    /// <summary>
    /// 核心领域方法：记录接收数量。
    /// 业务约束：默认不主动抛出异常以允许上层策略决定是否允许超收（例如电缆行业的称重容差）；若需要严格校验可在此处抛出业务异常。
    /// </summary>
    /// <param name="qty">本次接收数量。</param>
    public void AddReceivedQty(decimal qty)
    {
        if (ReceivedQty + qty > ExpectedQty)
        {
            // 可选：是否允许一定比例的超收（电缆行业由于称重误差，通常允许 3%~5% 的容差）
            // throw new Exception("超出采购单允许的接收数量！");
        }

        ReceivedQty += qty;
    }
}