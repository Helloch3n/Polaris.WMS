using Polaris.WMS.Outbound.Domain.Integration.Inventories;
using Polaris.WMS.Outbound.Domain.SalesOrders;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Polaris.WMS.Outbound.Domain.SalesShipments;

/// <summary>
/// 销售发货领域服务，负责实际发货执行与来源订单回写。
/// </summary>
public class SalesShipmentManager(
    IRepository<SalesShipment, Guid> salesShipmentRepository,
    IRepository<SalesShipmentRecord, Guid> salesShipmentRecordRepository,
    IRepository<SalesShipmentAllocation, Guid> salesShipmentAllocationRepository,
    IRepository<SalesOrder, Guid> salesOrderRepository)
    : DomainService
{
    private IExternalInventoryProvider ExternalInventoryProvider =>
        LazyServiceProvider.LazyGetRequiredService<IExternalInventoryProvider>();

    /// <summary>
    /// 向销售发货明细新增分配记录。
    /// </summary>
    public async Task<SalesShipmentAllocation> AddAllocationAsync(
        Guid salesShipmentId,
        Guid detailId,
        SalesShipmentAllocationCreateInfo allocation)
    {
        var queryable = await salesShipmentRepository.WithDetailsAsync(x => x.Details);
        var shipment = await AsyncExecuter.FirstOrDefaultAsync(queryable.Where(x => x.Id == salesShipmentId));
        if (shipment == null)
        {
            throw new BusinessException("销售发货单不存在。")
                .WithData("SalesShipmentId", salesShipmentId);
        }

        if (shipment.Status == OutboundOrderStatus.Completed)
        {
            throw new BusinessException("销售发货单已完成，不允许继续分配。")
                .WithData("SalesShipmentId", salesShipmentId)
                .WithData("Status", shipment.Status);
        }

        var detail = shipment.Details.FirstOrDefault(x => x.Id == detailId);
        if (detail == null)
        {
            throw new BusinessException("销售发货明细不存在。")
                .WithData("DetailId", detailId);
        }

        detail.Allocate(allocation.Qty);

        var entity = SalesShipmentAllocation.Create(
            allocation.Id,
            shipment.Id,
            detail.Id,
            detail.ProductId,
            detail.ProductCode,
            detail.ProductName,
            allocation.Qty,
            allocation.ContainerId,
            allocation.ContainerCode,
            allocation.SourceLocationId,
            allocation.SourceLocationCode,
            allocation.BatchNo,
            allocation.SN);

        shipment.RefreshStatus();
        await salesShipmentAllocationRepository.InsertAsync(entity, autoSave: true);
        await salesShipmentRepository.UpdateAsync(shipment, autoSave: true);

        return entity;
    }

    /// <summary>
    /// 释放销售发货明细的分配记录。
    /// </summary>
    public async Task RemoveAllocationAsync(Guid salesShipmentId, Guid detailId, Guid allocationId)
    {
        var queryable = await salesShipmentRepository.WithDetailsAsync(x => x.Details);
        var shipment = await AsyncExecuter.FirstOrDefaultAsync(queryable.Where(x => x.Id == salesShipmentId));
        if (shipment == null)
        {
            throw new BusinessException("销售发货单不存在。")
                .WithData("SalesShipmentId", salesShipmentId);
        }

        var detail = shipment.Details.FirstOrDefault(x => x.Id == detailId);
        if (detail == null)
        {
            throw new BusinessException("销售发货明细不存在。")
                .WithData("DetailId", detailId);
        }

        var allocation = await salesShipmentAllocationRepository.GetAsync(allocationId);
        if (allocation.SalesShipmentId != salesShipmentId || allocation.SalesShipmentDetailId != detailId)
        {
            throw new BusinessException("分配记录与销售发货明细不匹配。")
                .WithData("AllocationId", allocationId)
                .WithData("SalesShipmentId", salesShipmentId)
                .WithData("DetailId", detailId);
        }

        allocation.Release();
        detail.ReleaseAllocation(allocation.Qty);

        shipment.RefreshStatus();
        await salesShipmentAllocationRepository.UpdateAsync(allocation, autoSave: true);
        await salesShipmentRepository.UpdateAsync(shipment, autoSave: true);
    }

    /// <summary>
    /// 确认销售发货分配记录已完成拣货。
    /// </summary>
    public async Task<SalesShipmentAllocation> MarkAllocationPickedAsync(Guid salesShipmentId, Guid detailId, Guid allocationId)
    {
        var queryable = await salesShipmentRepository.WithDetailsAsync(x => x.Details);
        var shipment = await AsyncExecuter.FirstOrDefaultAsync(queryable.Where(x => x.Id == salesShipmentId));
        if (shipment == null)
        {
            throw new BusinessException("销售发货单不存在。")
                .WithData("SalesShipmentId", salesShipmentId);
        }

        var detail = shipment.Details.FirstOrDefault(x => x.Id == detailId);
        if (detail == null)
        {
            throw new BusinessException("销售发货明细不存在。")
                .WithData("DetailId", detailId);
        }

        var allocation = await salesShipmentAllocationRepository.GetAsync(allocationId);
        if (allocation.SalesShipmentId != salesShipmentId || allocation.SalesShipmentDetailId != detailId)
        {
            throw new BusinessException("分配记录与销售发货明细不匹配。")
                .WithData("AllocationId", allocationId)
                .WithData("SalesShipmentId", salesShipmentId)
                .WithData("DetailId", detailId);
        }

        allocation.MarkPicked();
        detail.MarkPicked(allocation.Qty);

        shipment.RefreshStatus();
        await salesShipmentAllocationRepository.UpdateAsync(allocation, autoSave: true);
        await salesShipmentRepository.UpdateAsync(shipment, autoSave: true);

        return allocation;
    }

    /// <summary>
    /// 向销售发货明细新增发货记录。
    /// </summary>
    public async Task AddRecordsAsync(
        Guid salesShipmentId,
        Guid detailId,
        IReadOnlyCollection<SalesShipmentRecordCreateInfo> records)
    {
        if (records == null || records.Count == 0)
        {
            throw new BusinessException("至少需要一条发货记录。");
        }

        var queryable = await salesShipmentRepository.WithDetailsAsync(x => x.Details);
        var shipment = await AsyncExecuter.FirstOrDefaultAsync(queryable.Where(x => x.Id == salesShipmentId));
        if (shipment == null)
        {
            throw new BusinessException("销售发货单不存在。")
                .WithData("SalesShipmentId", salesShipmentId);
        }

        var detail = shipment.Details.FirstOrDefault(x => x.Id == detailId);
        if (detail == null)
        {
            throw new BusinessException("销售发货明细不存在。")
                .WithData("DetailId", detailId);
        }

        foreach (var record in records)
        {
            detail.AddRecord(
                record.Id,
                record.Qty,
                record.ContainerId,
                record.ContainerCode,
                record.LocationId,
                record.LocationCode,
                record.BatchNo,
                record.SN);
        }

        await salesShipmentRepository.UpdateAsync(shipment, autoSave: true);
    }

    /// <summary>
    /// 审核并执行销售发货单。
    /// </summary>
    public async Task ApproveAndExecuteAsync(Guid salesShipmentId)
    {
        // 1. 加载单据及明细
        var queryable = await salesShipmentRepository.WithDetailsAsync(x => x.Details);
        var shipment = await AsyncExecuter.FirstOrDefaultAsync(queryable.Where(x => x.Id == salesShipmentId));
        if (shipment == null)
        {
            throw new BusinessException("销售发货单不存在。")
                .WithData("SalesShipmentId", salesShipmentId);
        }

        // 2. 校验基础状态
        if (!shipment.Details.Any())
        {
            throw new BusinessException("销售发货单至少需要一条明细，无法审核执行。")
                .WithData("SalesShipmentId", salesShipmentId);
        }

        if (shipment.Status == OutboundOrderStatus.Completed)
        {
            throw new BusinessException("当前销售发货单已完成，不允许重复审核执行。")
                .WithData("SalesShipmentId", salesShipmentId)
                .WithData("Status", shipment.Status);
        }

        SalesOrder? salesOrder = null;
        if (shipment.SourceSalesOrderId.HasValue)
        {
            var salesOrderQuery = await salesOrderRepository.WithDetailsAsync(x => x.Details);
            salesOrder = await AsyncExecuter.FirstOrDefaultAsync(salesOrderQuery.Where(x => x.Id == shipment.SourceSalesOrderId.Value));
            if (salesOrder == null)
            {
                throw new BusinessException("来源销售订单不存在，无法执行发货。")
                    .WithData("SourceSalesOrderId", shipment.SourceSalesOrderId.Value);
            }
        }

        // 3. 按明细执行出库
        foreach (var detail in shipment.Details)
        {
            if (detail.RequiredQty <= 0)
            {
                throw new BusinessException("销售发货明细数量必须大于0。")
                    .WithData("DetailId", detail.Id);
            }

            var records = await salesShipmentRecordRepository.GetListAsync(x => x.SalesShipmentDetailId == detail.Id);
            if (records.Count == 0)
            {
                throw new BusinessException("销售发货明细没有发货记录，无法审核执行。")
                    .WithData("DetailId", detail.Id);
            }

            var recordQtyTotal = records.Sum(x => x.Qty);
            if (recordQtyTotal != detail.RequiredQty)
            {
                throw new BusinessException("发货记录累计数量必须等于应发数量。")
                    .WithData("DetailId", detail.Id)
                    .WithData("RequiredQty", detail.RequiredQty)
                    .WithData("RecordQty", recordQtyTotal);
            }

            foreach (var record in records)
            {
                await ExternalInventoryProvider.ShipAsync(new ExternalMiscOutboundShipInfo
                {
                    OrderNo = shipment.ShipmentNo,
                    DetailId = record.Id,
                    ContainerId = record.ContainerId,
                    LocationId = record.LocationId,
                    ProductId = record.ProductId,
                    Qty = record.Qty,
                    BatchNo = record.BatchNo,
                    SN = record.SN
                });
            }

            detail.MarkShipped(recordQtyTotal);

            var pickedAllocations = await salesShipmentAllocationRepository.GetListAsync(x =>
                x.SalesShipmentDetailId == detail.Id &&
                x.Status == SalesShipmentAllocationStatus.Picked);

            foreach (var allocation in pickedAllocations)
            {
                allocation.MarkIssued();
                await salesShipmentAllocationRepository.UpdateAsync(allocation, autoSave: true);
            }

            if (salesOrder != null && detail.SourceSalesOrderLineId.HasValue)
            {
                salesOrder.ApplyShipment(detail.SourceSalesOrderLineId.Value, recordQtyTotal);
            }
        }

        // 4. 单据执行完成并持久化
        shipment.Execute();
        await salesShipmentRepository.UpdateAsync(shipment, autoSave: true);

        if (salesOrder != null)
        {
            await salesOrderRepository.UpdateAsync(salesOrder, autoSave: true);
        }
    }
}

public class SalesShipmentRecordCreateInfo
{
    public Guid Id { get; set; }
    public decimal Qty { get; set; }
    public Guid ContainerId { get; set; }
    public string ContainerCode { get; set; } = string.Empty;
    public Guid LocationId { get; set; }
    public string LocationCode { get; set; } = string.Empty;
    public string BatchNo { get; set; } = string.Empty;
    public string SN { get; set; } = string.Empty;
}

public class SalesShipmentAllocationCreateInfo
{
    public Guid Id { get; set; }
    public decimal Qty { get; set; }
    public Guid ContainerId { get; set; }
    public string ContainerCode { get; set; } = string.Empty;
    public Guid SourceLocationId { get; set; }
    public string SourceLocationCode { get; set; } = string.Empty;
    public string BatchNo { get; set; } = string.Empty;
    public string SN { get; set; } = string.Empty;
}


