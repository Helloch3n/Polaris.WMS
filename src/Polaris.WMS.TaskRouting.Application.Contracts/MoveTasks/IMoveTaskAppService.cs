using Volo.Abp.Application.Services;
using Polaris.WMS.TaskRouting.Application.Contracts.MoveTasks.Dtos;

namespace Polaris.WMS.TaskRouting.Application.Contracts.MoveTasks;

public interface IMoveTaskAppService : IApplicationService
{
    public Task CreateAsync(CreateMoveTaskDto input);
    public Task CompleteAsync(CompleteMoveTaskDto input);
}