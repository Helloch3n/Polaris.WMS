// MiscOutboundOrder.cs

using Polaris.WMS.Inventories.MiscOrders;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Polaris.WMS.Outbound.Domain.MiscOrders;

public class MiscOutboundOrder : FullAuditedAggregateRoot<Guid>
{
    public string OrderNo { get; private set; }
    public Guid AccountAliasId { get; private set; }
    public string AccountAliasDescription { get; private set; }
    public Guid CostCenterId { get; private set; }
    public string CostCenterCode { get; private set; }
    public string CostCenterName { get; private set; }
    public MiscOperationType Type { get; private set; }
    public MiscOrderStatus Status { get; private set; }
    public string? Remark { get; private set; }

    private readonly List<MiscOutboundOrderDetail> _details = new();
    public IReadOnlyCollection<MiscOutboundOrderDetail> Details => _details;

    protected MiscOutboundOrder()
    {
    }

    internal MiscOutboundOrder(
        Guid id,
        string orderNo,
        Guid accountAliasId,
        string accountAliasDescription,
        Guid costCenterId,
        string costCenterCode,
        string costCenterName,
        string? remark = null) : base(id)
    {
        OrderNo = Check.NotNullOrWhiteSpace(orderNo, nameof(orderNo), maxLength: 64);
        AccountAliasId = accountAliasId != Guid.Empty
            ? accountAliasId
            : throw new BusinessException("账户别名不能为空");
        AccountAliasDescription = Check.NotNullOrWhiteSpace(accountAliasDescription, nameof(accountAliasDescription), maxLength: 500);
        CostCenterId = costCenterId != Guid.Empty
            ? costCenterId
            : throw new BusinessException("成本中心不能为空");
        CostCenterCode = Check.NotNullOrWhiteSpace(costCenterCode, nameof(costCenterCode), maxLength: 50);
        CostCenterName = Check.NotNullOrWhiteSpace(costCenterName, nameof(costCenterName), maxLength: 200);
        Type = MiscOperationType.Outbound;
        Status = MiscOrderStatus.Draft;
        Remark = remark;
    }

    public static MiscOutboundOrder Create(
        Guid id,
        string orderNo,
        Guid accountAliasId,
        string accountAliasDescription,
        Guid costCenterId,
        string costCenterCode,
        string costCenterName,
        string? remark = null)
    {
        return new MiscOutboundOrder(
            id,
            orderNo,
            accountAliasId,
            accountAliasDescription,
            costCenterId,
            costCenterCode,
            costCenterName,
            remark);
    }

    public void UpdateHeader(
        Guid accountAliasId,
        string accountAliasDescription,
        Guid costCenterId,
        string costCenterCode,
        string costCenterName,
        string? remark)
    {
        EnsureDraft();
        AccountAliasId = accountAliasId != Guid.Empty
            ? accountAliasId
            : throw new BusinessException("账户别名不能为空");
        AccountAliasDescription = Check.NotNullOrWhiteSpace(accountAliasDescription, nameof(accountAliasDescription), maxLength: 500);
        CostCenterId = costCenterId != Guid.Empty
            ? costCenterId
            : throw new BusinessException("成本中心不能为空");
        CostCenterCode = Check.NotNullOrWhiteSpace(costCenterCode, nameof(costCenterCode), maxLength: 50);
        CostCenterName = Check.NotNullOrWhiteSpace(costCenterName, nameof(costCenterName), maxLength: 200);
        Remark = remark;
    }

    public MiscOutboundOrderDetail AddDetail(
        Guid detailId,
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
        EnsureDraft();

        var detail = new MiscOutboundOrderDetail(
            detailId,
            Id,
            warehouseId,
            warehouseCode,
            warehouseName,
            locationId,
            locationCode,
            containerId,
            containerCode,
            productId,
            productCode,
            productName,
            sn,
            batchNo,
            craftVersion,
            unit,
            qty,
            remark);

        _details.Add(detail);
        return detail;
    }

    public void UpdateDetail(
        Guid detailId,
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
        EnsureDraft();

        var detail = _details.FirstOrDefault(x => x.Id == detailId);
        if (detail == null)
        {
            throw new BusinessException("其他出库明细不存在").WithData("DetailId", detailId);
        }

        detail.Update(
            warehouseId,
            warehouseCode,
            warehouseName,
            locationId,
            locationCode,
            containerId,
            containerCode,
            productId,
            productCode,
            productName,
            sn,
            batchNo,
            craftVersion,
            unit,
            qty,
            remark);
    }

    public void RemoveDetail(Guid detailId)
    {
        EnsureDraft();

        var detail = _details.FirstOrDefault(x => x.Id == detailId);
        if (detail != null)
        {
            _details.Remove(detail);
        }
    }

    public void Execute()
    {
        EnsureDraft();

        if (_details.Count == 0)
        {
            throw new BusinessException("其他出库单至少需要一条明细");
        }

        Status = MiscOrderStatus.Executed;
    }

    private void EnsureDraft()
    {
        if (Status != MiscOrderStatus.Draft)
        {
            throw new BusinessException("仅草稿状态单据允许修改");
        }
    }
}

