namespace Polaris.WMS.InventoryManage.Application.Contracts.CycleCountOrders.Dtos;

public class SubmitCountResultInput
{
    public Guid OrderId { get; set; }
    public string ContainerCode { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public decimal CountedQty { get; set; }
}