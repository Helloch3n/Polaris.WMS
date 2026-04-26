using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace Polaris.WMS.Outbound.Domain.SalesShipments;

/// <summary>
/// 销售发货单聚合根。
/// 负责表达本次仓库执行的实际发货单据。
/// </summary>
public class SalesShipment : FullAuditedAggregateRoot<Guid>
{
    public string ShipmentNo { get; private set; }
    public Guid? SourceSalesOrderId { get; private set; }
    public string? SourceSalesOrderNo { get; private set; }
    public Guid CustomerId { get; private set; }
    public string CustomerCode { get; private set; }
    public string CustomerName { get; private set; }
    public OutboundOrderStatus Status { get; private set; }
    public string? DeliveryContactName { get; private set; }
    public string? DeliveryPhone { get; private set; }
    public string? DeliveryAddress { get; private set; }
    public string? Remark { get; private set; }

    private readonly List<SalesShipmentDetail> _details = new();
    public IReadOnlyCollection<SalesShipmentDetail> Details => _details;

    protected SalesShipment()
    {
        ShipmentNo = string.Empty;
        CustomerCode = string.Empty;
        CustomerName = string.Empty;
        Status = OutboundOrderStatus.Created;
    }

    internal SalesShipment(
        Guid id,
        string shipmentNo,
        Guid? sourceSalesOrderId,
        string? sourceSalesOrderNo,
        Guid customerId,
        string customerCode,
        string customerName,
        string? deliveryContactName,
        string? deliveryPhone,
        string? deliveryAddress,
        string? remark = null) : base(id)
    {
        ShipmentNo = Check.NotNullOrWhiteSpace(shipmentNo, nameof(shipmentNo), maxLength: 64);
        SourceSalesOrderId = sourceSalesOrderId;
        SourceSalesOrderNo = sourceSalesOrderNo?.Trim();
        CustomerId = customerId != Guid.Empty ? customerId : throw new BusinessException("客户不能为空。");
        CustomerCode = Check.NotNullOrWhiteSpace(customerCode, nameof(customerCode), maxLength: 50);
        CustomerName = Check.NotNullOrWhiteSpace(customerName, nameof(customerName), maxLength: 200);
        DeliveryContactName = deliveryContactName?.Trim();
        DeliveryPhone = deliveryPhone?.Trim();
        DeliveryAddress = deliveryAddress?.Trim();
        Remark = remark?.Trim();
        Status = OutboundOrderStatus.Created;
    }

    public static SalesShipment Create(
        Guid id,
        string shipmentNo,
        Guid? sourceSalesOrderId,
        string? sourceSalesOrderNo,
        Guid customerId,
        string customerCode,
        string customerName,
        string? deliveryContactName,
        string? deliveryPhone,
        string? deliveryAddress,
        string? remark = null)
    {
        return new SalesShipment(
            id,
            shipmentNo,
            sourceSalesOrderId,
            sourceSalesOrderNo,
            customerId,
            customerCode,
            customerName,
            deliveryContactName,
            deliveryPhone,
            deliveryAddress,
            remark);
    }

    public void UpdateHeader(
        Guid customerId,
        string customerCode,
        string customerName,
        string? deliveryContactName,
        string? deliveryPhone,
        string? deliveryAddress,
        string? remark)
    {
        EnsureCreated();
        CustomerId = customerId != Guid.Empty ? customerId : throw new BusinessException("客户不能为空。");
        CustomerCode = Check.NotNullOrWhiteSpace(customerCode, nameof(customerCode), maxLength: 50);
        CustomerName = Check.NotNullOrWhiteSpace(customerName, nameof(customerName), maxLength: 200);
        DeliveryContactName = deliveryContactName?.Trim();
        DeliveryPhone = deliveryPhone?.Trim();
        DeliveryAddress = deliveryAddress?.Trim();
        Remark = remark?.Trim();
    }

    public SalesShipmentDetail AddDetail(
        Guid detailId,
        Guid? sourceSalesOrderLineId,
        int lineNo,
        Guid productId,
        string productCode,
        string productName,
        string unit,
        decimal requiredQty,
        string? remark = null)
    {
        EnsureCreated();

        if (_details.Any(x => x.LineNo == lineNo))
        {
            throw new BusinessException("销售发货单已存在重复行号。")
                .WithData("LineNo", lineNo);
        }

        var detail = new SalesShipmentDetail(
            detailId,
            Id,
            sourceSalesOrderLineId,
            lineNo,
            productId,
            productCode,
            productName,
            unit,
            requiredQty,
            remark);

        _details.Add(detail);
        return detail;
    }

    public void RemoveDetail(Guid detailId)
    {
        EnsureCreated();

        var detail = _details.FirstOrDefault(x => x.Id == detailId);
        if (detail != null)
        {
            _details.Remove(detail);
        }
    }

    public void Execute()
    {
        EnsureExecutable();

        if (!_details.Any())
        {
            throw new BusinessException("销售发货单至少需要一条明细。");
        }

        if (_details.Any(x => x.ShippedQty < x.RequiredQty))
        {
            throw new BusinessException("销售发货单存在未完成发货的明细，无法执行完成。")
                .WithData("ShipmentNo", ShipmentNo);
        }

        Status = OutboundOrderStatus.Completed;
    }

    public void RefreshStatus()
    {
        if (!_details.Any())
        {
            Status = OutboundOrderStatus.Created;
            return;
        }

        if (_details.All(x => x.ShippedQty >= x.RequiredQty))
        {
            Status = OutboundOrderStatus.Completed;
            return;
        }

        if (_details.Any(x => x.ShippedQty > 0))
        {
            Status = OutboundOrderStatus.Shipped;
            return;
        }

        if (_details.Any(x => x.PickedQty > 0))
        {
            Status = OutboundOrderStatus.Picking;
            return;
        }

        if (_details.All(x => x.AllocatedQty >= x.RequiredQty))
        {
            Status = OutboundOrderStatus.Allocated;
            return;
        }

        if (_details.Any(x => x.AllocatedQty > 0))
        {
            Status = OutboundOrderStatus.PartiallyAllocated;
            return;
        }

        Status = OutboundOrderStatus.Created;
    }

    private void EnsureCreated()
    {
        if (Status != OutboundOrderStatus.Created)
        {
            throw new BusinessException("仅新建状态的销售发货单允许修改。")
                .WithData("Status", Status);
        }
    }

    private void EnsureExecutable()
    {
        if (Status == OutboundOrderStatus.Completed)
        {
            throw new BusinessException("当前销售发货单已完成，不允许重复执行。")
                .WithData("Status", Status);
        }
    }
}

public class SalesShipmentDetail : Entity<Guid>
{
    public Guid SalesShipmentId { get; private set; }
    public Guid? SourceSalesOrderLineId { get; private set; }
    public int LineNo { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductCode { get; private set; }
    public string ProductName { get; private set; }
    public string Unit { get; private set; }
    public decimal RequiredQty { get; private set; }
    public decimal AllocatedQty { get; private set; }
    public decimal PickedQty { get; private set; }
    public decimal ShippedQty { get; private set; }
    public string? Remark { get; private set; }

    private readonly List<SalesShipmentRecord> _records = new();
    public IReadOnlyCollection<SalesShipmentRecord> Records => _records;

    protected SalesShipmentDetail()
    {
        ProductCode = string.Empty;
        ProductName = string.Empty;
        Unit = string.Empty;
    }

    internal SalesShipmentDetail(
        Guid id,
        Guid salesShipmentId,
        Guid? sourceSalesOrderLineId,
        int lineNo,
        Guid productId,
        string productCode,
        string productName,
        string unit,
        decimal requiredQty,
        string? remark = null) : base(id)
    {
        SalesShipmentId = salesShipmentId;
        SourceSalesOrderLineId = sourceSalesOrderLineId;
        LineNo = lineNo;
        ProductId = productId != Guid.Empty ? productId : throw new BusinessException("物料不能为空。");
        ProductCode = Check.NotNullOrWhiteSpace(productCode, nameof(productCode), maxLength: 50);
        ProductName = Check.NotNullOrWhiteSpace(productName, nameof(productName), maxLength: 200);
        Unit = Check.NotNullOrWhiteSpace(unit, nameof(unit), maxLength: 20);
        RequiredQty = requiredQty > 0 ? requiredQty : throw new BusinessException("发货数量必须大于0。").WithData("RequiredQty", requiredQty);
        AllocatedQty = 0;
        PickedQty = 0;
        ShippedQty = 0;
        Remark = remark?.Trim();
    }

    public void Allocate(decimal qty)
    {
        if (qty <= 0)
        {
            throw new BusinessException("分配数量必须大于0。");
        }

        if (AllocatedQty + qty > RequiredQty)
        {
            throw new BusinessException("累计分配数量不能大于应发数量。")
                .WithData("RequiredQty", RequiredQty)
                .WithData("AllocatedQty", AllocatedQty)
                .WithData("IncrementQty", qty);
        }

        AllocatedQty += qty;
    }

    public void ReleaseAllocation(decimal qty)
    {
        if (qty <= 0)
        {
            throw new BusinessException("释放分配数量必须大于0。");
        }

        if (AllocatedQty - qty < PickedQty)
        {
            throw new BusinessException("释放后分配数量不能小于已拣数量。")
                .WithData("AllocatedQty", AllocatedQty)
                .WithData("PickedQty", PickedQty)
                .WithData("ReleaseQty", qty);
        }

        AllocatedQty -= qty;
    }

    public void MarkPicked(decimal qty)
    {
        if (qty <= 0)
        {
            throw new BusinessException("已拣数量必须大于0。");
        }

        if (PickedQty + qty > AllocatedQty)
        {
            throw new BusinessException("累计已拣数量不能大于分配数量。")
                .WithData("AllocatedQty", AllocatedQty)
                .WithData("PickedQty", PickedQty)
                .WithData("IncrementQty", qty);
        }

        PickedQty += qty;
    }

    public SalesShipmentRecord AddRecord(
        Guid id,
        decimal qty,
        Guid containerId,
        string containerCode,
        Guid locationId,
        string locationCode,
        string batchNo,
        string sn)
    {
        if (qty <= 0)
        {
            throw new BusinessException("发货记录数量必须大于0。");
        }

        if (containerId == Guid.Empty)
        {
            throw new BusinessException("发货记录的容器Id不能为空。");
        }

        if (locationId == Guid.Empty)
        {
            throw new BusinessException("发货记录的库位Id不能为空。");
        }

        var nextQty = _records.Sum(x => x.Qty) + qty;
        if (nextQty > RequiredQty)
        {
            throw new BusinessException("发货记录累计数量不能大于应发数量。")
                .WithData("RequiredQty", RequiredQty)
                .WithData("CurrentRecordQty", _records.Sum(x => x.Qty))
                .WithData("IncrementQty", qty);
        }

        var record = new SalesShipmentRecord(
            id,
            SalesShipmentId,
            Id,
            SourceSalesOrderLineId,
            ProductId,
            ProductCode,
            ProductName,
            qty,
            containerId,
            containerCode,
            locationId,
            locationCode,
            batchNo,
            sn);

        _records.Add(record);
        return record;
    }

    public void MarkShipped(decimal qty)
    {
        if (qty <= 0)
        {
            throw new BusinessException("实发数量必须大于0。");
        }

        if (ShippedQty + qty > RequiredQty)
        {
            throw new BusinessException("累计实发数量不能大于应发数量。")
                .WithData("RequiredQty", RequiredQty)
                .WithData("ShippedQty", ShippedQty)
                .WithData("IncrementQty", qty);
        }

        if (PickedQty > 0 && ShippedQty + qty > PickedQty)
        {
            throw new BusinessException("累计实发数量不能大于已拣数量。")
                .WithData("PickedQty", PickedQty)
                .WithData("ShippedQty", ShippedQty)
                .WithData("IncrementQty", qty);
        }

        ShippedQty += qty;
    }
}

public class SalesShipmentAllocation : FullAuditedAggregateRoot<Guid>
{
    public Guid SalesShipmentId { get; private set; }
    public Guid SalesShipmentDetailId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductCode { get; private set; }
    public string ProductName { get; private set; }
    public decimal Qty { get; private set; }
    public Guid ContainerId { get; private set; }
    public string ContainerCode { get; private set; }
    public Guid SourceLocationId { get; private set; }
    public string SourceLocationCode { get; private set; }
    public string BatchNo { get; private set; }
    public string SN { get; private set; }
    public SalesShipmentAllocationStatus Status { get; private set; }

    protected SalesShipmentAllocation()
    {
        ProductCode = string.Empty;
        ProductName = string.Empty;
        ContainerCode = string.Empty;
        SourceLocationCode = string.Empty;
        BatchNo = string.Empty;
        SN = string.Empty;
    }

    internal SalesShipmentAllocation(
        Guid id,
        Guid salesShipmentId,
        Guid salesShipmentDetailId,
        Guid productId,
        string productCode,
        string productName,
        decimal qty,
        Guid containerId,
        string containerCode,
        Guid sourceLocationId,
        string sourceLocationCode,
        string batchNo,
        string sn) : base(id)
    {
        SalesShipmentId = salesShipmentId != Guid.Empty ? salesShipmentId : throw new BusinessException("销售发货单Id不能为空。");
        SalesShipmentDetailId = salesShipmentDetailId != Guid.Empty ? salesShipmentDetailId : throw new BusinessException("销售发货明细Id不能为空。");
        ProductId = productId != Guid.Empty ? productId : throw new BusinessException("物料不能为空。");
        ProductCode = Check.NotNullOrWhiteSpace(productCode, nameof(productCode), maxLength: 50);
        ProductName = Check.NotNullOrWhiteSpace(productName, nameof(productName), maxLength: 200);
        Qty = qty > 0 ? qty : throw new BusinessException("分配数量必须大于0。");
        ContainerId = containerId != Guid.Empty ? containerId : throw new BusinessException("分配容器Id不能为空。");
        ContainerCode = Check.NotNullOrWhiteSpace(containerCode, nameof(containerCode), maxLength: 64);
        SourceLocationId = sourceLocationId != Guid.Empty ? sourceLocationId : throw new BusinessException("分配库位Id不能为空。");
        SourceLocationCode = Check.NotNullOrWhiteSpace(sourceLocationCode, nameof(sourceLocationCode), maxLength: 64);
        BatchNo = Check.NotNullOrWhiteSpace(batchNo, nameof(batchNo), maxLength: 100);
        SN = Check.NotNullOrWhiteSpace(sn, nameof(sn), maxLength: 100);
        Status = SalesShipmentAllocationStatus.Reserved;
    }

    public static SalesShipmentAllocation Create(
        Guid id,
        Guid salesShipmentId,
        Guid salesShipmentDetailId,
        Guid productId,
        string productCode,
        string productName,
        decimal qty,
        Guid containerId,
        string containerCode,
        Guid sourceLocationId,
        string sourceLocationCode,
        string batchNo,
        string sn)
    {
        return new SalesShipmentAllocation(
            id,
            salesShipmentId,
            salesShipmentDetailId,
            productId,
            productCode,
            productName,
            qty,
            containerId,
            containerCode,
            sourceLocationId,
            sourceLocationCode,
            batchNo,
            sn);
    }

    public void MarkTaskCreated()
    {
        if (Status != SalesShipmentAllocationStatus.Reserved)
        {
            throw new BusinessException("仅已分配状态允许创建拣货任务。")
                .WithData("Status", Status);
        }

        Status = SalesShipmentAllocationStatus.TaskCreated;
    }

    public void MarkPicked()
    {
        if (Status != SalesShipmentAllocationStatus.Reserved && Status != SalesShipmentAllocationStatus.TaskCreated)
        {
            throw new BusinessException("当前分配状态不允许确认已拣。")
                .WithData("Status", Status);
        }

        Status = SalesShipmentAllocationStatus.Picked;
    }

    public void Release()
    {
        if (Status == SalesShipmentAllocationStatus.Picked || Status == SalesShipmentAllocationStatus.Issued)
        {
            throw new BusinessException("已拣或已出库的分配记录不允许释放。")
                .WithData("Status", Status);
        }

        if (Status == SalesShipmentAllocationStatus.Released)
        {
            return;
        }

        Status = SalesShipmentAllocationStatus.Released;
    }

    public void MarkIssued()
    {
        if (Status != SalesShipmentAllocationStatus.Picked)
        {
            throw new BusinessException("仅已拣状态允许确认出库。")
                .WithData("Status", Status);
        }

        Status = SalesShipmentAllocationStatus.Issued;
    }
}

public class SalesShipmentRecord : Entity<Guid>
{
    public Guid SalesShipmentId { get; private set; }
    public Guid SalesShipmentDetailId { get; private set; }
    public Guid? SourceSalesOrderLineId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductCode { get; private set; }
    public string ProductName { get; private set; }
    public decimal Qty { get; private set; }
    public Guid ContainerId { get; private set; }
    public string ContainerCode { get; private set; }
    public Guid LocationId { get; private set; }
    public string LocationCode { get; private set; }
    public string BatchNo { get; private set; }
    public string SN { get; private set; }

    protected SalesShipmentRecord()
    {
        ProductCode = string.Empty;
        ProductName = string.Empty;
        ContainerCode = string.Empty;
        LocationCode = string.Empty;
        BatchNo = string.Empty;
        SN = string.Empty;
    }

    internal SalesShipmentRecord(
        Guid id,
        Guid salesShipmentId,
        Guid salesShipmentDetailId,
        Guid? sourceSalesOrderLineId,
        Guid productId,
        string productCode,
        string productName,
        decimal qty,
        Guid containerId,
        string containerCode,
        Guid locationId,
        string locationCode,
        string batchNo,
        string sn) : base(id)
    {
        SalesShipmentId = salesShipmentId;
        SalesShipmentDetailId = salesShipmentDetailId;
        SourceSalesOrderLineId = sourceSalesOrderLineId;
        ProductId = productId;
        ProductCode = Check.NotNullOrWhiteSpace(productCode, nameof(productCode), maxLength: 50);
        ProductName = Check.NotNullOrWhiteSpace(productName, nameof(productName), maxLength: 200);
        Qty = qty > 0 ? qty : throw new BusinessException("发货记录数量必须大于0。");
        ContainerId = containerId;
        ContainerCode = Check.NotNullOrWhiteSpace(containerCode, nameof(containerCode), maxLength: 64);
        LocationId = locationId;
        LocationCode = Check.NotNullOrWhiteSpace(locationCode, nameof(locationCode), maxLength: 64);
        BatchNo = Check.NotNullOrWhiteSpace(batchNo, nameof(batchNo), maxLength: 100);
        SN = Check.NotNullOrWhiteSpace(sn, nameof(sn), maxLength: 100);
    }
}


