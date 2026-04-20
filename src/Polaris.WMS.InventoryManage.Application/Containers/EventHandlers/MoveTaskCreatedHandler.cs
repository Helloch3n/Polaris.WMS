using Polaris.WMS.InventoryManage.Domain.Containers;
using Polaris.WMS.Tasks.MoveTask.Events;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;

namespace Polaris.WMS.InventoryManage.Application.Containers.EventHandlers;

public class MoveTaskCreatedHandler(IRepository<Container, Guid> containerRepository)
    : ILocalEventHandler<MoveTaskCreatedEto>, ITransientDependency
{
    public async Task HandleEventAsync(MoveTaskCreatedEto eventData)
    {
        // 如果搬运任务有关联容器/盘具
        if (eventData.ContainerId.HasValue)
        {
            // 1. 查找对应的盘具（假设 ContainerId 对应 Container 的主键或关联键）
            var container = await containerRepository.FindAsync(eventData.ContainerId.Value);

            if (container != null)
            {
                // 2. 调用实体的锁定方法（符合 DDD 充血模型）
                container.Lock("搬运任务已创建，盘具锁定中");

                await containerRepository.UpdateAsync(container);
            }
        }
    }
}