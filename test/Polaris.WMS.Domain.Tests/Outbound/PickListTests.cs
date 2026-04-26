using System;
using System.Linq;
using Polaris.WMS.Outbound.Domain.PickLists;
using Shouldly;
using Xunit;

namespace Polaris.WMS.Outbound;

public class PickListTests
{
    [Fact]
    public void AttachTask_And_MarkLinePicked_Should_Refresh_PickList_Status()
    {
        var pickList = CreatePickList();
        var line = pickList.Lines.Single();

        pickList.AttachTask(line.Id, Guid.NewGuid(), "MOV-20260425-0001");
        pickList.Status.ShouldBe(PickListStatus.TaskCreated);

        pickList.MarkLinePicked(line.MoveTaskId!.Value, Guid.NewGuid());
        pickList.Status.ShouldBe(PickListStatus.Picked);
        pickList.Lines.Single().IsPicked.ShouldBeTrue();
    }   

    [Fact]
    public void AttachTask_Should_Reject_Duplicate_Task_Binding()
    {
        var pickList = CreatePickList();
        var line = pickList.Lines.Single();
        pickList.AttachTask(line.Id, Guid.NewGuid(), "MOV-20260425-0001");

        Should.Throw<Volo.Abp.BusinessException>(() =>
            pickList.AttachTask(line.Id, Guid.NewGuid(), "MOV-20260425-0002"));
    }

    private static PickList CreatePickList()
    {
        var pickList = PickList.Create(
            Guid.NewGuid(),
            "PKL-20260425-0001",
            Guid.NewGuid(),
            Guid.NewGuid(),
            "STAGE-01",
            "测试拣货单");

        pickList.AddLine(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "SHP-001",
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "SKU-001",
            "测试物料",
            5,
            Guid.NewGuid(),
            "CTN-001",
            Guid.NewGuid(),
            "LOC-A01",
            "BATCH-001",
            "SN-001");

        return pickList;
    }
}

