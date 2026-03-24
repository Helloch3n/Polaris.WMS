namespace Polaris.WMS.Outbound
{
    public enum OutboundOrderStatus
    {
        Created = 0,
        PartiallyAllocated = 1,
        Allocated = 2,
        Picking = 3,
        Shipped = 4,
        Completed = 5
    }
}
