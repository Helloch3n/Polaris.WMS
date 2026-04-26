using Polaris.WMS.Outbound.Domain;
using Polaris.WMS.Tasks;
using Polaris.WMS.Tasks.MoveTask.Events;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;

namespace Polaris.WMS.Outbound.Application.PickLists.EventHandlers;

public class PickTaskCompletedEventHandler(
    OutboundFulfillmentManager outboundFulfillmentManager)
    : ILocalEventHandler<MoveTaskCompletedEto>, ITransientDependency
{
    public async Task HandleEventAsync(MoveTaskCompletedEto eventData)
    {
        if (eventData.TaskType != MoveTaskType.PickDown)
        {
            return;
        }

        await outboundFulfillmentManager.HandlePickTaskCompletedAsync(eventData.TaskId, eventData.ActualLocationId);
    }
}

