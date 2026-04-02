using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Polaris.WMS.Inbound.Domain.PurchaseOrders;

public class PurchaseReceiptManager(IRepository<PurchaseOrder, Guid> poRepository) : DomainService
{
    /// <summary>
    /// Track A: 核心领域逻辑 - 执行基于 ASN 的收货
    /// 自动将数量穿透同步到源头的 PO 聚合根，并驱动状态机。
    /// </summary>
    public async Task ExecuteAsnReceiptAsync(Asns.AdvancedShippingNotice asn, string scmAsnRowNo, decimal actualQty, decimal tolerancePercentage = 0.05m)
    {
        // 1. 找到 ASN 明细
        var asnDetail = asn.Details.FirstOrDefault(x => x.ScmAsnRowNo == scmAsnRowNo);
        if (asnDetail == null) throw new UserFriendlyException($"在 ASN 单 {asn.AsnNo} 中未找到行号 {scmAsnRowNo}");

        // 2. 找到对应的 PO 聚合根
        var po = await poRepository.FirstOrDefaultAsync(x => x.PoNo == asnDetail.SourcePoNo);
        if (po == null) throw new UserFriendlyException($"溯源失败：未找到关联的采购订单 {asnDetail.SourcePoNo}");

        // 3. 找到 PO 明细
        var poDetail = po.Details.FirstOrDefault(x => x.LineNo == asnDetail.SourcePoLineNo);
        if (poDetail == null) throw new UserFriendlyException($"溯源失败：采购订单 {po.PoNo} 中无行号 {asnDetail.SourcePoLineNo}");

        // 4. 执行核心防呆扣减 (这里的顺序很重要，如果 PO 层超收容差校验失败，会直接抛出异常阻断事务)
        poDetail.AddReceivedQty(actualQty, tolerancePercentage); // PO 层校验容差
        asnDetail.AddReceivedQty(actualQty);                     // ASN 层直接累加

        // 5. 驱动状态机流转
        po.RefreshStatus();
        asn.RefreshStatus();

        // 6. 更新实体
        await poRepository.UpdateAsync(po);
    }

    /// <summary>
    /// Track B: 核心领域逻辑 - 执行基于 PO 的无码盲收
    /// </summary>
    public Task ExecutePoReceiptAsync(PurchaseOrder po, int lineNo, decimal actualQty, decimal tolerancePercentage = 0.05m)
    {
        var poDetail = po.Details.FirstOrDefault(x => x.LineNo == lineNo);
        if (poDetail == null) throw new UserFriendlyException($"采购订单 {po.PoNo} 中未找到行号 {lineNo}");

        // 执行容差累加
        poDetail.AddReceivedQty(actualQty, tolerancePercentage);

        // 状态机流转
        po.RefreshStatus();

        return Task.CompletedTask;
    }
}