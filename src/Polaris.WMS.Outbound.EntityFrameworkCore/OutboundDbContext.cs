using Microsoft.EntityFrameworkCore;
using Polaris.WMS.Outbound.Domain.Handovers;
using Polaris.WMS.Outbound.Domain.MiscOrders;
using Polaris.WMS.Outbound.Domain.PickLists;
using Polaris.WMS.Outbound.Domain.Reviews;
using Polaris.WMS.Outbound.Domain.SalesOrders;
using Polaris.WMS.Outbound.Domain.SalesShipments;
using Polaris.WMS.Outbound.Domain.WaveOrders;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Polaris.WMS.Outbound.EntityFrameworkCore;

// 指定连接字符串的名字。默认叫 "Default"，
// 如果未来主数据要用独立数据库，只需要在 appsettings.json 里加一个 "MasterData" 的连接串即可。
[ConnectionStringName("Default")]
public class OutboundDbContext : AbpDbContext<OutboundDbContext>,IOutBoundDbContext
{
    // 声明当前模块管辖的实体
    public DbSet<MiscOutboundOrder> MiscOutboundOrders { get; set; }
    public DbSet<MiscOutboundOrderDetail> MiscOutboundOrderDetails { get; set; }
    public DbSet<SalesOrder> SalesOrders { get; set; }
    public DbSet<SalesOrderDetail> SalesOrderDetails { get; set; }
    public DbSet<SalesShipment> SalesShipments { get; set; }
    public DbSet<SalesShipmentDetail> SalesShipmentDetails { get; set; }
    public DbSet<SalesShipmentAllocation> SalesShipmentAllocations { get; set; }
    public DbSet<SalesShipmentRecord> SalesShipmentRecords { get; set; }
    public DbSet<WaveOrder> WaveOrders { get; set; }
    public DbSet<WaveOrderLine> WaveOrderLines { get; set; }
    public DbSet<PickList> PickLists { get; set; }
    public DbSet<PickListLine> PickListLines { get; set; }
    public DbSet<OutboundReviewOrder> OutboundReviewOrders { get; set; }
    public DbSet<OutboundReviewLine> OutboundReviewLines { get; set; }
    public DbSet<OutboundHandoverOrder> OutboundHandoverOrders { get; set; }
    public DbSet<OutboundHandoverLine> OutboundHandoverLines { get; set; }


    public OutboundDbContext(DbContextOptions<OutboundDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureOutBound();
    }
}