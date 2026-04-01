using Polaris.WMS.InventoryManage.Domain.Reels;
using Polaris.WMS.Tasks.MoveTask.Events;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus;

namespace Polaris.WMS.InventoryManage.Application.Reels.EventHandlers;

public class MoveTaskCompletedHandler(
    ReelManager reelManager,
    IRepository<Reel, Guid> reelRepository)
    : ILocalEventHandler<MoveTaskCompletedEto>, ITransientDependency
{
    public async Task HandleEventAsync(MoveTaskCompletedEto eventData)
    {
        await reelManager.TransferLocationAsync(
            eventData.ContainerId,
            eventData.ActualLocationId
        );
    }
}