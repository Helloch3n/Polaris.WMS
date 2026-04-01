using System.ComponentModel.DataAnnotations;

namespace Polaris.WMS.InventoryManage.Application.Contracts.MaterialFeeding.Dtos;

public class UnlockMaterialInputDto
{
    [Required] public string ContainerCode { get; set; } // 扫码或MES传入的盘号

    [Required] public string OrderNo { get; set; } // 当前正在结束的生产工单号（用于安全校验）
}