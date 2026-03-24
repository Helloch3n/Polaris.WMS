namespace Polaris.WMS.Tasks;

public enum MoveTaskType
{
    /// <summary> 入库上架 (从月台/机台搬到正式货架) </summary>
    Putaway = 10,

    /// <summary> 移库送检 (从机台搬到质检区) </summary>
    MoveToQc = 20,

    /// <summary> 库内理货 (正式库区之间的同状态移动) </summary>
    InternalMove = 30,

    /// <summary> 拣货下架 (从货架搬到发货暂存区) </summary>
    PickDown = 40
}