using Microsoft.EntityFrameworkCore;
using Polaris.WMS.Outbound.Domain.MiscOrders;
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