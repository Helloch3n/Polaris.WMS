using Volo.Abp.Application.Dtos;

namespace Polaris.WMS.MasterData.Application.Contracts.AccountAliases.Dtos;

public class AccountAliasSearchDto : PagedAndSortedResultRequestDto
{
    public string? Alias { get; set; }
    public string? Description { get; set; }
    public bool? IsActiveAtNow { get; set; }
}

