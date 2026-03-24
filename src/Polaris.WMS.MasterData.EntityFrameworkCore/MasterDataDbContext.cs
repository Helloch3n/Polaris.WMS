using Microsoft.EntityFrameworkCore;
using Polaris.WMS.MasterData.Domain.Locations;
using Polaris.WMS.MasterData.Domain.Zones;
using Polaris.WMS.Users;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Polaris.WMS.MasterData.EntityFrameworkCore;

// 指定连接字符串的名字。默认叫 "Default"，
// 如果未来主数据要用独立数据库，只需要在 appsettings.json 里加一个 "MasterData" 的连接串即可。
[ConnectionStringName("Default")]
public class MasterDataDbContext : AbpDbContext<MasterDataDbContext>
{
    // 声明当前模块管辖的实体
    public DbSet<Location> WarehouseLocations { get; set; }
    public DbSet<Zone> WarehouseZones { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<UserWarehouse> UserWarehouses { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }

    public MasterDataDbContext(DbContextOptions<MasterDataDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureMasterData();
    }
}