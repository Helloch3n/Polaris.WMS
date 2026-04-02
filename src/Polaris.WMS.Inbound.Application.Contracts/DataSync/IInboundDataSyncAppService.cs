using Polaris.WMS.Inbound.Application.Contracts.DataSync.Dtos;
using Volo.Abp.Application.Services;

namespace Polaris.WMS.Inbound.Application.Contracts.DataSync;

public interface IInboundDataSyncAppService : IApplicationService
{
    /// <summary>
    /// 同步 (Upsert) 采购订单
    /// </summary>
    Task SyncPoAsync(SyncPoDto input);

    /// <summary>
    /// 同步 (Upsert) 提前发货通知单
    /// </summary>
    Task SyncAsnAsync(SyncAsnDto input);
}