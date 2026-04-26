using System;
using System.Linq;
using Polaris.WMS.Outbound.Domain.SalesShipments;
using Shouldly;
using Volo.Abp;
using Xunit;

namespace Polaris.WMS.Outbound;

public class SalesShipmentAllocationTests
{
    [Fact]
    public void RefreshStatus_Should_Follow_Allocation_And_Picking_Progress()
    {
        var shipment = CreateShipment();
        var detail = shipment.Details.Single();

        detail.Allocate(4);
        shipment.RefreshStatus();
        shipment.Status.ShouldBe(OutboundOrderStatus.PartiallyAllocated);

        detail.Allocate(6);
        shipment.RefreshStatus();
        shipment.Status.ShouldBe(OutboundOrderStatus.Allocated);

        detail.MarkPicked(5);
        shipment.RefreshStatus();
        shipment.Status.ShouldBe(OutboundOrderStatus.Picking);
    }

    [Fact]
    public void ReleaseAllocation_Should_Not_Drop_Below_PickedQty()
    {
        var shipment = CreateShipment();
        var detail = shipment.Details.Single();

        detail.Allocate(10);
        detail.MarkPicked(4);

        Action action = () => detail.ReleaseAllocation(7);
        Should.Throw<BusinessException>(action);
        detail.AllocatedQty.ShouldBe(10);
        detail.PickedQty.ShouldBe(4);
    }

    [Fact]
    public void MarkShipped_Should_Not_Exceed_PickedQty_When_PickingExists()
    {
        var shipment = CreateShipment();
        var detail = shipment.Details.Single();

        detail.Allocate(10);
        detail.MarkPicked(6);

        Action action = () => detail.MarkShipped(7);
        Should.Throw<BusinessException>(action);
        detail.ShippedQty.ShouldBe(0);
        detail.PickedQty.ShouldBe(6);
    }

    private static SalesShipment CreateShipment()
    {
        var shipment = SalesShipment.Create(
            Guid.NewGuid(),
            "SO-TEST-001",
            Guid.NewGuid(),
            "SO-0001",
            Guid.NewGuid(),
            "CUST-001",
            "测试客户",
            "张三",
            "13800000000",
            "上海市浦东新区",
            "测试发货单");

        shipment.AddDetail(
            Guid.NewGuid(),
            Guid.NewGuid(),
            1,
            Guid.NewGuid(),
            "SKU-001",
            "测试物料",
            "PCS",
            10,
            "测试明细");

        return shipment;
    }
}



