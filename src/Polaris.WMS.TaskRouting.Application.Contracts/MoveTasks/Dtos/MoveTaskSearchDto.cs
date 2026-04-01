using Volo.Abp.Application.Dtos;

namespace Polaris.WMS.TaskRouting.Application.Contracts.MoveTasks.Dtos;

public class MoveTaskSearchDto : PagedAndSortedResultRequestDto
{
    public string? TaskCode { get; set; }
}