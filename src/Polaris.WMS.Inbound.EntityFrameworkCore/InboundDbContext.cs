using Microsoft.EntityFrameworkCore;
using Polaris.WMS.Inbound.Domain.Asns;
using Polaris.WMS.Inbound.Domain.MiscOrders;
using Polaris.WMS.Inbound.Domain.ProductionInbounds;
using Polaris.WMS.Inbound.Domain.PurchaseOrders;
using Polaris.WMS.Inbound.Domain.PurchaseReceipts;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Polaris.WMS.Inbound.EntityFrameworkCore;

// 指定连接字符串的名字。默认叫 "Default"，
// 如果未来主数据要用独立数据库，只需要在 appsettings.json 里加一个 "MasterData" 的连接串即可。
[ConnectionStringName("Default")]
public class InboundDbContext : AbpDbContext<InboundDbContext>,IInboundDbContext
{
    // 声明当前模块管辖的实体
    public DbSet<ProductionInbound> ProductionInbounds { get; set; }
    public DbSet<ProductionInboundDetail> ProductionInboundDetails { get; set; }
    public DbSet<PurchaseOrder>  PurchaseOrders { get; set; }
    public DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
    public DbSet<PurchaseReceipt> PurchaseReceipts { get; set; }
    public DbSet<PurchaseReceiptDetail> PurchaseReceiptDetails { get; set; }
    public DbSet<PurchaseRecord> PurchaseRecords { get; set; }
    public DbSet<AdvancedShippingNotice> Asns { get; set; }
    public DbSet<AsnDetail> AsnDetails { get; set; }
    public DbSet<MiscInboundOrder> MiscInboundOrders { get; set; }
    public DbSet<MiscInboundOrderDetail> MiscInboundOrderDetails { get; set; }

    public InboundDbContext(DbContextOptions<InboundDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureInbound();
    }
}