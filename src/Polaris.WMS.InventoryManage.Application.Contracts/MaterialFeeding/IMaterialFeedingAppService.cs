using Polaris.WMS.InventoryManage.Application.Contracts.MaterialFeeding.Dtos;
using Volo.Abp.Application.Services;

namespace Polaris.WMS.InventoryManage.Application.Contracts.MaterialFeeding;

public interface IMaterialFeedingAppService : IApplicationService
{
    public Task FeedAndLockAsync(FeedMaterialInputDto input);

    public Task UnlockAndReturnAsync(UnlockMaterialInputDto input);

    public Task ConsumeAsync(ConsumeMaterialInputDto input);
}