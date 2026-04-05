using Polaris.WMS.Asns;
using Polaris.WMS.Inbound.Application.Contracts.DataSync;
using Polaris.WMS.Inbound.Application.Contracts.DataSync.Dtos;
using Polaris.WMS.Inbound.Domain.Asns;
using Polaris.WMS.Inbound.Domain.PurchaseOrders;
using Polaris.WMS.MasterData.Application.Contracts.Integration.Products;
using Polaris.WMS.MasterData.Application.Contracts.Integration.Suppliers;
using Polaris.WMS.PurchaseOrders;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Polaris.WMS.Inbound.Application.DataSync;

/// <summary>
/// 负责与外部系统（SCM 等）进行入库相关数据同步的应用服务。
/// 仅实现必要的 Upsert/同步逻辑：新增/更新采购单与 ASN。
/// 注意：本类仅做数据同步与防呆校验，不包含复杂的业务编排。
/// </summary>
public class InboundDataSyncAppService(
    IRepository<PurchaseOrder, Guid> poRepository,
    IRepository<AdvancedShippingNotice, Guid> asnRepository,
    ISupplierIntegrationService supplierIntegrationService,
    IProductIntegrationService productIntegrationService) : ApplicationService, IInboundDataSyncAppService
{
    /// <summary>
    /// 同步采购单（PO）。支持新增和更新（严格防呆）。
    /// 规则要点：
    /// - 如果 PO 不存在则新增（并关联物料主数据）；
    /// - 如果 PO 已存在且处于 Open 状态，则允许更新主信息与明细；
    /// - 当 PO 已进入收货流程（非 Open）时，拒绝 SCM 侧修改。
    /// </summary>
    public async Task SyncPoAsync(SyncPoDto input)
    {
        // 尝试根据 PO 号查找现有单据
        var existingPo = await poRepository.FirstOrDefaultAsync(x => x.PoNo == input.PoNo);

        // 根据供应商编码获取供应商主数据（外部集成）
        var supplierInfo = await supplierIntegrationService.GetSupplierInfoByCodeAsync(input.SupplierCode);

        if (existingPo == null)
        {
            // 场景：新增采购单（Insert）
            var newPo = new PurchaseOrder(
                GuidGenerator.Create(), input.PoNo, supplierInfo.Id, input.SupplierCode, input.SupplierName,
                input.OrderDate,
                input.ExpectedDeliveryDate);

            // 从外部产品服务获取物料主数据并创建明细
            foreach (var item in input.Details)
            {
                var productInfo = await productIntegrationService.GetProductInfoByCodeAsync(item.ProductCode);
                newPo.AddDetail(GuidGenerator.Create(), item.LineNo, productInfo.Id, item.ProductCode, item.ProductName,
                    item.UoM, item.ExpectedQty, item.IsQualityCheckRequired);
            }

            await poRepository.InsertAsync(newPo);
        }
        else
        {
            // 场景：更新采购单（Update）——仅在单据仍为 Open 时允许修改
            if (existingPo.Status != PurchaseOrderStatus.Open)
            {
                throw new UserFriendlyException($"采购单 {input.PoNo} 状态为 {existingPo.Status}，仓库已开始收货作业，禁止 SCM 修改！");
            }

            // 更新主表基础信息
            existingPo.UpdateBasicInfo(input.OrderDate, input.ExpectedDeliveryDate);

            // Upsert 明细：不存在则新增，存在则更新期望数量（并校验不小于已收数量）
            foreach (var item in input.Details)
            {
                var existingDetail = existingPo.Details.FirstOrDefault(x => x.LineNo == item.LineNo);
                if (existingDetail == null)
                {
                    var productInfo = await productIntegrationService.GetProductInfoByCodeAsync(item.ProductCode);
                    existingPo.AddDetail(GuidGenerator.Create(), item.LineNo, productInfo.Id, item.ProductCode,
                        item.ProductName, item.UoM, item.ExpectedQty, item.IsQualityCheckRequired);
                }
                else
                {
                    // 防呆校验：期望数量不得小于已接收数量
                    if (item.ExpectedQty < existingDetail.ReceivedQty)
                    {
                        throw new UserFriendlyException(
                            $"修改失败：行号 {item.LineNo} 的期望数量 {item.ExpectedQty} 不能小于 WMS 已实收的数量 {existingDetail.ReceivedQty}！");
                    }

                    existingPo.UpdateDetailExpectedQty(item.LineNo, item.ExpectedQty);
                }
            }

            await poRepository.UpdateAsync(existingPo);
        }
    }

    /// <summary>
    /// 同步 ASN（Advanced Shipping Notice）。支持新增与更新（带防呆）。
    /// 规则要点：
    /// - 新增：创建 ASN 及其明细，并关联物料主数据；
    /// - 更新：仅在 ASN 处于 Pending 状态时允许 SCM 修改明细；
    /// - 当 ASN 已到达并开始收货时，拒绝 SCM 修改并提示人工干预。
    /// </summary>
    public async Task SyncAsnAsync(SyncAsnDto input)
    {
        // 查找是否已存在相同 ASN 号
        var existingAsn = await asnRepository.FirstOrDefaultAsync(x => x.AsnNo == input.AsnNo);
        var supplierInfo = await supplierIntegrationService.GetSupplierInfoByCodeAsync(input.SupplierCode);

        if (existingAsn == null)
        {
            // 新增 ASN
            var newAsn = new AdvancedShippingNotice(
                GuidGenerator.Create(), input.AsnNo, supplierInfo.Id, input.SupplierCode, input.SupplierName,
                input.ExpectedArrivalTime);

            foreach (var item in input.Details)
            {
                var po=await poRepository.GetAsync(x=>x.PoNo==item.SourcePoNo);
                if (po == null)
                {
                    throw new UserFriendlyException($"ASN 明细行 {item.ScmAsnRowNo} 中的源头采购单号 {item.SourcePoNo} 在系统中不存在！");
                }

                var productInfo = await productIntegrationService.GetProductInfoByCodeAsync(item.ProductCode);
                newAsn.AddDetail(GuidGenerator.Create(), item.ScmAsnRowNo,po.Id,item.SourcePoNo, item.SourcePoLineNo,
                    productInfo.Id, item.ProductCode, item.ProductName, item.UoM, item.ExpectedQty,
                    item.SupplierBatchNo,
                    input.LicensePlate);
            }

            await asnRepository.InsertAsync(newAsn);
        }
        else
        {
            // 更新 ASN：仅允许在 Pending 状态下进行修改
            if (existingAsn.Status != AsnStatus.Pending)
            {
                throw new UserFriendlyException(
                    $"ASN [{input.AsnNo}] 的送货车辆已到达月台并开始收货 (状态:{existingAsn.Status})。禁止 SCM 远程修改！请联系工厂仓管进行人工异常处理。");
            }

            // 1) 更新主表基础信息
            existingAsn.UpdateBasicInfo(input.ExpectedArrivalTime);
            // 2) 明细 Upsert：存在则更新，不存在则新增
            foreach (var item in input.Details)
            {
                
                var existingDetail = existingAsn.Details.FirstOrDefault(x => x.ScmAsnRowNo == item.ScmAsnRowNo);

                if (existingDetail == null)
                {
                    var po= await poRepository.GetAsync(x=>x.PoNo==item.SourcePoNo);
                    if (po == null)
                    {
                        throw new UserFriendlyException($"ASN 明细行 {item.ScmAsnRowNo} 中的源头采购单号 {item.SourcePoNo} 在系统中不存在！");
                    }
                    var productInfo = await productIntegrationService.GetProductInfoByCodeAsync(item.ProductCode);

                    existingAsn.AddDetail(
                        GuidGenerator.Create(),
                        item.ScmAsnRowNo,
                        po.Id,
                        item.SourcePoNo,
                        item.SourcePoLineNo,
                        productInfo.Id,
                        item.ProductCode,
                        productInfo.Name,
                        item.UoM,
                        item.ExpectedQty,
                        item.SupplierBatchNo,
                        input.LicensePlate);
                }
                else
                {
                    // 更新期望数量、批次与车牌信息
                    existingAsn.UpdateDetailInfo(
                        item.ScmAsnRowNo,
                        item.ExpectedQty,
                        item.SupplierBatchNo,
                        input.LicensePlate);
                }
            }

            // 3) 删除 SCM 侧已移除的明细行（安全清理）
            var incomingRowNos = input.Details.Select(x => x.ScmAsnRowNo).ToList();
            existingAsn.RemoveDetailsNotIn(incomingRowNos);
            await asnRepository.UpdateAsync(existingAsn);
        }
    }
}