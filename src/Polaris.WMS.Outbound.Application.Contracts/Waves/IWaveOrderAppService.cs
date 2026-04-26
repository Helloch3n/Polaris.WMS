using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Polaris.WMS.Outbound.Application.Contracts.Waves;

public interface IWaveOrderAppService : IApplicationService
{
    /// <summary>
    /// 创建出库波次单。
    /// </summary>
    Task<WaveOrderDto> CreateAsync(CreateWaveOrderDto input);

    /// <summary>
    /// 获取出库波次单详情。
    /// </summary>
    Task<WaveOrderDto> GetAsync(Guid id);

    /// <summary>
    /// 分页获取出库波次单列表。
    /// </summary>
    Task<PagedResultDto<WaveOrderDto>> GetListAsync(WaveOrderSearchDto input);
}

