using Volo.Abp;
using Volo.Abp.Application.Services;

namespace Polaris.WMS.InventoryManage.Application.Contracts.Integration.Reels;

[RemoteService(IsEnabled = false)]
public interface IReelIntegrationService : IApplicationService
{
    Task<ReelIntegrationDto> GetAsync(Guid id);

    Task TransferLocationAsync(Guid containerId, Guid scannedLocationId, string? orderNo = null);

    Task<List<ReelIntegrationDto>> GetReelByLocationIdAsync(Guid locationId);
}