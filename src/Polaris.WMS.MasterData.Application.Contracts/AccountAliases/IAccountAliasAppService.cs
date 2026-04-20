using Polaris.WMS.MasterData.Application.Contracts.AccountAliases.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Polaris.WMS.MasterData.Application.Contracts.AccountAliases;

public interface IAccountAliasAppService : IApplicationService
{
    /// <summary>
    /// 创建账户别名。
    /// </summary>
    Task<AccountAliasDto> CreateAsync(CreateAccountAliasDto input);

    /// <summary>
    /// 获取账户别名详情。
    /// </summary>
    Task<AccountAliasDto> GetAsync(Guid id);

    /// <summary>
    /// 分页获取账户别名列表。
    /// </summary>
    Task<PagedResultDto<AccountAliasDto>> GetListAsync(AccountAliasSearchDto input);
}


