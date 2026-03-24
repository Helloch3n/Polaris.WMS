using Polaris.WMS.InventoryManage.Application.Contracts.Integration.Reels;
using Polaris.WMS.InventoryManage.Domain.Reels;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Polaris.WMS.InventoryManage.Application.Integration.Reels;

[RemoteService(IsEnabled = false)]
public class ReelIntegrationService(
    ReelManager reelManager,
    IRepository<Reel, Guid> reelRepository
) : ApplicationService, IReelIntegrationService
{
    public async Task TransferLocationAsync(Guid containerId, Guid scannedLocationId, string? orderNo = null)
    {
        await reelManager.TransferLocationAsync(containerId, scannedLocationId);
    }

    public async Task<List<ReelIntegrationDto>> GetReelByLocationIdAsync(Guid locationId)
    {
        // 1. 直接使用 ABP 泛型仓储的条件查询获取列表
        // 假设你的仓储变量名叫 reelRepository
        var reels = await reelRepository.GetListAsync(x => x.Id == locationId);

        // 2. 防护：如果没有查到任何盘具，直接返回空集合
        if (reels == null || !reels.Any())
        {
            return new List<ReelIntegrationDto>();
        }

        // 3. 映射为集成 DTO 并返回
        return reels.Select(r => new ReelIntegrationDto
        {
            Id = r.Id,
            LocationId = r.CurrentLocationId
        }).ToList();
    }

    public async Task<ReelIntegrationDto> GetAsync(Guid id)
    {
        var reel = await reelRepository.FirstOrDefaultAsync(x => x.Id == id);
        if (reel == null) return null;

        return new ReelIntegrationDto
        {
            Id = reel.Id,
            ReelNo = reel.ReelNo
        };
    }
}