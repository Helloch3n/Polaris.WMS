using Polaris.WMS.MasterData.Application.Contracts.CostCenters.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Polaris.WMS.MasterData.Application.Contracts.CostCenters;

public interface ICostCenterAppService : IApplicationService
{
    /// <summary>
    /// 创建成本中心。
    /// </summary>
    Task<CostCenterDto> CreateAsync(CreateCostCenterDto input);

    /// <summary>
    /// 获取成本中心详情。
    /// </summary>
    Task<CostCenterDto> GetAsync(Guid id);

    /// <summary>
    /// 分页获取成本中心列表。
    /// </summary>
    Task<PagedResultDto<CostCenterDto>> GetListAsync(CostCenterSearchDto input);
}

