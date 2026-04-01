using System.ComponentModel.DataAnnotations;

namespace Polaris.WMS.InventoryManage.Application.Contracts.MaterialFeeding.Dtos;

public class FeedMaterialInputDto
{
    [Required] public string ContainerCode { get; set; } // 扫码得到的盘号

    [Required] public string OrderNo { get; set; } // 当前机台的生产工单号

    // 可选：如果需要严格校验PDA扫的库位和系统记录是否一致
    //public string LocationCode { get; set; }
}