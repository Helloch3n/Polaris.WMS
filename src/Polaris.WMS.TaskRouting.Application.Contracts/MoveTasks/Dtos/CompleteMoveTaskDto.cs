using System.ComponentModel.DataAnnotations;

namespace Polaris.WMS.TaskRouting.Application.Contracts.MoveTasks.Dtos;

public class CompleteMoveTaskDto
{
    [Required(ErrorMessage = "库位编码不能为空")] public Guid TaskId { get; set; }

    /// <summary>
    /// 实际放下的库位编码(手持机扫描得到)
    /// </summary>
    [Required]
    public string ScannedLocationCode { get; set; }
}