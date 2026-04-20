using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace Polaris.WMS.Inbound.Domain.MiscOrders;

public class MiscInboundOrderDetail : Entity<Guid>
{
    public Guid MiscInboundOrderId { get; private set; }
    public Guid WarehouseId { get; private set; }
    public string WarehouseCode { get; private set; }
    public string WarehouseName { get; private set; }
    public Guid LocationId { get; private set; }
    public string LocationCode { get; private set; }
    public Guid ContainerId { get; private set; }
    public string ContainerCode { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductCode { get; private set; }
    public string ProductName { get; private set; }
    public string SN { get; private set; }
    public string BatchNo { get; private set; }
    public string? CraftVersion { get; private set; }
    public string Unit { get; private set; }
    public decimal Qty { get; private set; }
    public string? Remark { get; private set; }

    protected MiscInboundOrderDetail()
    {
    }

    internal MiscInboundOrderDetail(
        Guid id,
        Guid miscInboundOrderId,
        Guid warehouseId,
        string warehouseCode,
        string warehouseName,
        Guid locationId,
        string locationCode,
        Guid containerId,
        string containerCode,
        Guid productId,
        string productCode,
        string productName,
        string sn,
        string batchNo,
        string? craftVersion,
        string unit,
        decimal qty,
        string? remark = null) : base(id)
    {
        MiscInboundOrderId = miscInboundOrderId;
        WarehouseId = warehouseId != Guid.Empty
            ? warehouseId
            : throw new BusinessException("仓库不能为空");
        WarehouseCode = Check.NotNullOrWhiteSpace(warehouseCode, nameof(warehouseCode), maxLength: 64);
        WarehouseName = Check.NotNullOrWhiteSpace(warehouseName, nameof(warehouseName), maxLength: 200);
        LocationId = locationId != Guid.Empty
            ? locationId
            : throw new BusinessException("库位不能为空");
        LocationCode = Check.NotNullOrWhiteSpace(locationCode, nameof(locationCode), maxLength: 64);
        ContainerId = containerId != Guid.Empty
            ? containerId
            : throw new BusinessException("盘具不能为空");
        ContainerCode = Check.NotNullOrWhiteSpace(containerCode, nameof(containerCode), maxLength: 64);
        ProductId = productId != Guid.Empty
            ? productId
            : throw new BusinessException("物料不能为空");
        ProductCode = Check.NotNullOrWhiteSpace(productCode, nameof(productCode), maxLength: 50);
        ProductName = Check.NotNullOrWhiteSpace(productName, nameof(productName), maxLength: 200);
        SN = Check.NotNullOrWhiteSpace(sn, nameof(sn), maxLength: 100);
        BatchNo = Check.NotNullOrWhiteSpace(batchNo, nameof(batchNo), maxLength: 100);
        CraftVersion = craftVersion;
        Unit = Check.NotNullOrWhiteSpace(unit, nameof(unit), maxLength: 20);
        Qty = qty > 0
            ? qty
            : throw new BusinessException("数量必须大于0").WithData("Qty", qty);
        Remark = remark;
    }

    internal void Update(
        Guid warehouseId,
        string warehouseCode,
        string warehouseName,
        Guid locationId,
        string locationCode,
        Guid containerId,
        string containerCode,
        Guid productId,
        string productCode,
        string productName,
        string sn,
        string batchNo,
        string? craftVersion,
        string unit,
        decimal qty,
        string? remark = null)
    {
        WarehouseId = warehouseId != Guid.Empty
            ? warehouseId
            : throw new BusinessException("仓库不能为空");
        WarehouseCode = Check.NotNullOrWhiteSpace(warehouseCode, nameof(warehouseCode), maxLength: 64);
        WarehouseName = Check.NotNullOrWhiteSpace(warehouseName, nameof(warehouseName), maxLength: 200);
        LocationId = locationId != Guid.Empty
            ? locationId
            : throw new BusinessException("库位不能为空");
        LocationCode = Check.NotNullOrWhiteSpace(locationCode, nameof(locationCode), maxLength: 64);
        ContainerId = containerId != Guid.Empty
            ? containerId
            : throw new BusinessException("盘具不能为空");
        ContainerCode = Check.NotNullOrWhiteSpace(containerCode, nameof(containerCode), maxLength: 64);
        ProductId = productId != Guid.Empty
            ? productId
            : throw new BusinessException("物料不能为空");
        ProductCode = Check.NotNullOrWhiteSpace(productCode, nameof(productCode), maxLength: 50);
        ProductName = Check.NotNullOrWhiteSpace(productName, nameof(productName), maxLength: 200);
        SN = Check.NotNullOrWhiteSpace(sn, nameof(sn), maxLength: 100);
        BatchNo = Check.NotNullOrWhiteSpace(batchNo, nameof(batchNo), maxLength: 100);
        CraftVersion = craftVersion;
        Unit = Check.NotNullOrWhiteSpace(unit, nameof(unit), maxLength: 20);
        Qty = qty > 0
            ? qty
            : throw new BusinessException("数量必须大于0").WithData("Qty", qty);
        Remark = remark;
    }
}