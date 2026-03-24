using Polaris.WMS.InventoryManage.Application.Contracts.Inventories.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Polaris.WMS.InventoryManage.Application.Contracts.Inventories
{
    public interface IInventoryAppService : IApplicationService
    {
        Task<PagedResultDto<InventoryDto>> GetListAsync(InventorySearchDto input);
        //Task<InventoryDto> ProductionReceiveAsync(ProductionReceiveInput input);
    }
}

