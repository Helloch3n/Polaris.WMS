using Polaris.WMS.InventoryManage.Domain.Containers;
using Polaris.WMS.Tasks.MoveTask.Events;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus;

namespace Polaris.WMS.InventoryManage.Application.Containers.EventHandlers;

public class MoveTaskCompletedHandler(
    ContainerManager containerManager,
    IRepository<Container, Guid> containerRepository)
    : ILocalEventHandler<MoveTaskCompletedEto>, ITransientDependency
{
    public async Task HandleEventAsync(MoveTaskCompletedEto eventData)
    {
        await containerManager.TransferLocationAsync(
            eventData.ContainerId,
            eventData.ActualLocationId
        );
    }
}