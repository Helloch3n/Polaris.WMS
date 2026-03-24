using Polaris.WMS.BillNumbers;
using Polaris.WMS.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Polaris.WMS.TaskRouting.Domain.MoveTasks;

public class MoveTaskManager(
    IBillNumberGenerator billNumberGenerator,
    IRepository<MoveTask, Guid> transferOrderRepository) : DomainService
{
    public async Task<MoveTask> CreateMoveTaskAsync(MoveTaskType taskType, Guid containerId,
        Guid? sourceLocationId,
        Guid targetLocationId)
    {
        var orderNo = await billNumberGenerator.GetNextNumberAsync("MOV");
        var normalizedOrderNo = Check.NotNullOrWhiteSpace(orderNo, nameof(orderNo));

        var exists = await transferOrderRepository.AnyAsync(x => x.TaskNo == normalizedOrderNo);
        if (exists)
        {
            throw new BusinessException("搬运任务号已存在")
                .WithData("搬运任务号", normalizedOrderNo);
        }

        return MoveTask.Create(Guid.NewGuid(), orderNo, taskType, containerId, sourceLocationId,
            targetLocationId);
    }
}