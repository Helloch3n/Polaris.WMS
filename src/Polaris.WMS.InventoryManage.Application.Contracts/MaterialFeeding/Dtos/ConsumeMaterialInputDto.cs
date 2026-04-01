using System.ComponentModel.DataAnnotations;

namespace Polaris.WMS.InventoryManage.Application.Contracts.MaterialFeeding.Dtos;

public class ConsumeMaterialInputDto
{
    [Required] public string ContainerCode { get; set; } // 托盘号/容器号 (必填，定位物理位置)

    [Required] public string OrderNo { get; set; } // 工单号 (必填)

    [Required] public decimal ConsumeQty { get; set; } // 消耗总量 (必填)

    public string? InventorySN { get; set; } // 单品唯一码/箱码 (可选)

    public string? BatchNo { get; set; } // 批次号 (可选)
}