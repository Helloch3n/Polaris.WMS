using Polaris.WMS.BillNumbers;
using Polaris.WMS.Outbound.Domain.Handovers;
using Polaris.WMS.Outbound.Domain.PickLists;
using Polaris.WMS.Outbound.Domain.Reviews;
using Polaris.WMS.Outbound.Domain.SalesShipments;
using Polaris.WMS.Outbound.Domain.WaveOrders;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Polaris.WMS.Outbound.Domain;

/// <summary>
/// 出库履约领域服务，负责波次、拣货、复核与交接闭环编排。
/// </summary>
public class OutboundFulfillmentManager(
    IRepository<WaveOrder, Guid> waveOrderRepository,
    IRepository<PickList, Guid> pickListRepository,
    IRepository<OutboundReviewOrder, Guid> outboundReviewOrderRepository,
    IRepository<OutboundHandoverOrder, Guid> outboundHandoverOrderRepository,
    IRepository<SalesShipment, Guid> salesShipmentRepository,
    IRepository<SalesShipmentAllocation, Guid> salesShipmentAllocationRepository,
    IBillNumberGenerator billNumberGenerator,
    SalesShipmentManager salesShipmentManager)
    : DomainService
{
    /// <summary>
    /// 根据销售发货单创建波次单。
    /// </summary>
    public async Task<WaveOrder> CreateWaveAsync(IReadOnlyCollection<Guid> salesShipmentIds, string? remark = null)
    {
        if (salesShipmentIds == null || salesShipmentIds.Count == 0)
        {
            throw new BusinessException("至少需要一张销售发货单才能创建波次单。");
        }

        var waveNo = await billNumberGenerator.GetNextNumberAsync("WAV");
        var wave = WaveOrder.Create(GuidGenerator.Create(), waveNo, remark);

        var query = await salesShipmentRepository.WithDetailsAsync(x => x.Details);
        var shipments = await AsyncExecuter.ToListAsync(query.Where(x => salesShipmentIds.Contains(x.Id)));
        if (shipments.Count != salesShipmentIds.Count)
        {
            throw new BusinessException("存在销售发货单不存在，无法创建波次单。");
        }

        foreach (var shipment in shipments)
        {
            foreach (var detail in shipment.Details)
            {
                wave.AddLine(
                    GuidGenerator.Create(),
                    shipment.Id,
                    shipment.ShipmentNo,
                    detail.Id,
                    shipment.CustomerId,
                    shipment.CustomerCode,
                    shipment.CustomerName,
                    detail.ProductId,
                    detail.ProductCode,
                    detail.ProductName,
                    detail.RequiredQty);
            }
        }

        await waveOrderRepository.InsertAsync(wave, autoSave: true);
        return wave;
    }

    /// <summary>
    /// 根据波次单创建拣货单。
    /// </summary>
    public async Task<PickList> CreatePickListAsync(Guid waveOrderId, Guid targetLocationId, string targetLocationCode, string? remark = null)
    {
        var waveQuery = await waveOrderRepository.WithDetailsAsync(x => x.Lines);
        var wave = await AsyncExecuter.FirstOrDefaultAsync(waveQuery.Where(x => x.Id == waveOrderId));
        if (wave == null)
        {
            throw new BusinessException("波次单不存在。")
                .WithData("WaveOrderId", waveOrderId);
        }

        if (!wave.Lines.Any())
        {
            throw new BusinessException("波次单没有明细，无法创建拣货单。");
        }

        var pickNo = await billNumberGenerator.GetNextNumberAsync("PKL");
        var pickList = PickList.Create(GuidGenerator.Create(), pickNo, wave.Id, targetLocationId, targetLocationCode, remark);

        var allocationList = await salesShipmentAllocationRepository.GetListAsync(x =>
            wave.Lines.Select(line => line.SalesShipmentDetailId).Contains(x.SalesShipmentDetailId) &&
            x.Status != SalesShipmentAllocationStatus.Released &&
            x.Status != SalesShipmentAllocationStatus.Issued);

        foreach (var waveLine in wave.Lines)
        {
            var matchedAllocations = allocationList
                .Where(x => x.SalesShipmentId == waveLine.SalesShipmentId && x.SalesShipmentDetailId == waveLine.SalesShipmentDetailId)
                .ToList();

            if (!matchedAllocations.Any())
            {
                throw new BusinessException("波次明细未找到有效分配记录，无法生成拣货单。")
                    .WithData("SalesShipmentDetailId", waveLine.SalesShipmentDetailId);
            }

            foreach (var allocation in matchedAllocations)
            {
                pickList.AddLine(
                    GuidGenerator.Create(),
                    allocation.SalesShipmentId,
                    waveLine.SalesShipmentNo,
                    allocation.SalesShipmentDetailId,
                    allocation.Id,
                    allocation.ProductId,
                    allocation.ProductCode,
                    allocation.ProductName,
                    allocation.Qty,
                    allocation.ContainerId,
                    allocation.ContainerCode,
                    allocation.SourceLocationId,
                    allocation.SourceLocationCode,
                    allocation.BatchNo,
                    allocation.SN);
            }
        }

        wave.Release();
        await pickListRepository.InsertAsync(pickList, autoSave: true);
        await waveOrderRepository.UpdateAsync(wave, autoSave: true);
        return pickList;
    }

    /// <summary>
    /// 根据拣货单创建复核单。
    /// </summary>
    public async Task<OutboundReviewOrder> CreateReviewAsync(Guid pickListId, string? remark = null)
    {
        var pickQuery = await pickListRepository.WithDetailsAsync(x => x.Lines);
        var pickList = await AsyncExecuter.FirstOrDefaultAsync(pickQuery.Where(x => x.Id == pickListId));
        if (pickList == null)
        {
            throw new BusinessException("拣货单不存在。")
                .WithData("PickListId", pickListId);
        }

        if (pickList.Status != PickListStatus.Picked)
        {
            throw new BusinessException("仅已完成拣货的拣货单允许创建复核单。")
                .WithData("Status", pickList.Status);
        }

        var reviewNo = await billNumberGenerator.GetNextNumberAsync("RVW");
        var review = OutboundReviewOrder.Create(GuidGenerator.Create(), reviewNo, pickList.Id, remark);

        foreach (var line in pickList.Lines)
        {
            review.AddLine(
                GuidGenerator.Create(),
                line.Id,
                line.SalesShipmentId,
                line.SalesShipmentDetailId,
                line.SalesShipmentAllocationId,
                line.ProductId,
                line.ProductCode,
                line.ProductName,
                line.Qty,
                line.ContainerId,
                line.ContainerCode,
                line.TargetLocationId,
                line.TargetLocationCode,
                line.BatchNo,
                line.SN);
        }

        await outboundReviewOrderRepository.InsertAsync(review, autoSave: true);
        return review;
    }

    /// <summary>
    /// 完成复核单。
    /// </summary>
    public async Task CompleteReviewAsync(Guid reviewId)
    {
        var review = await outboundReviewOrderRepository.GetAsync(reviewId);
        review.Complete();
        await outboundReviewOrderRepository.UpdateAsync(review, autoSave: true);
    }

    /// <summary>
    /// 根据复核单创建交接单。
    /// </summary>
    public async Task<OutboundHandoverOrder> CreateHandoverAsync(Guid reviewId, string? remark = null)
    {
        var reviewQuery = await outboundReviewOrderRepository.WithDetailsAsync(x => x.Lines);
        var review = await AsyncExecuter.FirstOrDefaultAsync(reviewQuery.Where(x => x.Id == reviewId));
        if (review == null)
        {
            throw new BusinessException("复核单不存在。")
                .WithData("ReviewId", reviewId);
        }

        if (review.Status != OutboundReviewStatus.Completed)
        {
            throw new BusinessException("仅已完成复核的复核单允许创建交接单。")
                .WithData("Status", review.Status);
        }

        var handoverNo = await billNumberGenerator.GetNextNumberAsync("HDO");
        var handover = OutboundHandoverOrder.Create(GuidGenerator.Create(), handoverNo, review.Id, remark);

        foreach (var line in review.Lines)
        {
            handover.AddLine(
                GuidGenerator.Create(),
                line.Id,
                line.SalesShipmentId,
                line.SalesShipmentDetailId,
                line.SalesShipmentAllocationId,
                line.ProductId,
                line.ProductCode,
                line.ProductName,
                line.Qty,
                line.ContainerId,
                line.ContainerCode,
                line.LocationId,
                line.LocationCode,
                line.BatchNo,
                line.SN);
        }

        await outboundHandoverOrderRepository.InsertAsync(handover, autoSave: true);
        return handover;
    }

    /// <summary>
    /// 完成交接单并触发销售发货执行。
    /// </summary>
    public async Task CompleteHandoverAsync(Guid handoverId)
    {
        var handoverQuery = await outboundHandoverOrderRepository.WithDetailsAsync(x => x.Lines);
        var handover = await AsyncExecuter.FirstOrDefaultAsync(handoverQuery.Where(x => x.Id == handoverId));
        if (handover == null)
        {
            throw new BusinessException("交接单不存在。")
                .WithData("HandoverId", handoverId);
        }

        handover.Complete();

        var lineGroups = handover.Lines.GroupBy(x => new { x.SalesShipmentId, x.SalesShipmentDetailId });
        foreach (var lineGroup in lineGroups)
        {
            var records = lineGroup.Select(line => new SalesShipmentRecordCreateInfo
            {
                Id = GuidGenerator.Create(),
                Qty = line.Qty,
                ContainerId = line.ContainerId,
                ContainerCode = line.ContainerCode,
                LocationId = line.LocationId,
                LocationCode = line.LocationCode,
                BatchNo = line.BatchNo,
                SN = line.SN
            }).ToList();

            await salesShipmentManager.AddRecordsAsync(lineGroup.Key.SalesShipmentId, lineGroup.Key.SalesShipmentDetailId, records);
        }

        foreach (var salesShipmentId in handover.Lines.Select(x => x.SalesShipmentId).Distinct())
        {
            await salesShipmentManager.ApproveAndExecuteAsync(salesShipmentId);
        }

        var review = await outboundReviewOrderRepository.GetAsync(handover.OutboundReviewOrderId);
        var pickList = await pickListRepository.GetAsync(review.PickListId);
        var wave = await waveOrderRepository.GetAsync(pickList.WaveOrderId);
        if (wave.Status != WaveOrderStatus.Completed && wave.Status != WaveOrderStatus.Cancelled)
        {
            wave.Complete();
            await waveOrderRepository.UpdateAsync(wave, autoSave: true);
        }

        await outboundHandoverOrderRepository.UpdateAsync(handover, autoSave: true);
    }

    /// <summary>
    /// 根据拣货任务完成事件回写拣货单和发货分配状态。
    /// </summary>
    public async Task HandlePickTaskCompletedAsync(Guid moveTaskId, Guid actualLocationId)
    {
        var pickQuery = await pickListRepository.WithDetailsAsync(x => x.Lines);
        var pickLists = await AsyncExecuter.ToListAsync(pickQuery.Where(x => x.Lines.Any(line => line.MoveTaskId == moveTaskId)));
        var pickList = pickLists.FirstOrDefault();
        if (pickList == null)
        {
            return;
        }

        var line = pickList.Lines.First(x => x.MoveTaskId == moveTaskId);
        if (line.IsPicked)
        {
            return;
        }

        pickList.MarkLinePicked(moveTaskId, actualLocationId);
        await pickListRepository.UpdateAsync(pickList, autoSave: true);

        var allocation = await salesShipmentAllocationRepository.GetAsync(line.SalesShipmentAllocationId);
        if (allocation.Status == SalesShipmentAllocationStatus.Picked || allocation.Status == SalesShipmentAllocationStatus.Issued)
        {
            return;
        }

        await salesShipmentManager.MarkAllocationPickedAsync(line.SalesShipmentId, line.SalesShipmentDetailId, line.SalesShipmentAllocationId);
    }
}



