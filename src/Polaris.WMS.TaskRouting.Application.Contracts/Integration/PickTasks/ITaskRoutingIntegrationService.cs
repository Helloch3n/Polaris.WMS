using Volo.Abp;
using Volo.Abp.Application.Services;

namespace Polaris.WMS.TaskRouting.Application.Contracts.Integration.PickTasks;

[RemoteService(IsEnabled = false)]
public interface ITaskRoutingIntegrationService : IApplicationService
{
    /// <summary>
    /// 创建拣货下架任务。
    /// </summary>
    Task<PickTaskIntegrationDto> CreatePickDownTaskAsync(CreatePickDownTaskInput input);
}

