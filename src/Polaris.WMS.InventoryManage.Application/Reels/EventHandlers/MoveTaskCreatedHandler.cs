using Polaris.WMS.InventoryManage.Domain.Reels;
using Polaris.WMS.Tasks.MoveTask.Events;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;

namespace Polaris.WMS.InventoryManage.Application.Reels.EventHandlers;

public class MoveTaskCreatedHandler(IRepository<Reel, Guid> reelRepository)
    : ILocalEventHandler<MoveTaskCreatedEto>, ITransientDependency
{
    public async Task HandleEventAsync(MoveTaskCreatedEto eventData)
    {
        // 如果搬运任务有关联容器/盘具
        if (eventData.ContainerId.HasValue)
        {
            // 1. 查找对应的盘具（假设 ContainerId 对应 Reel 的主键或关联键）
            var reel = await reelRepository.FindAsync(eventData.ContainerId.Value);

            if (reel != null)
            {
                // 2. 调用实体的锁定方法（符合 DDD 充血模型）
                reel.Lock("搬运任务已创建，盘具锁定中");

                await reelRepository.UpdateAsync(reel);
            }
        }
    }
}