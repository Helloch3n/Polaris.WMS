using Microsoft.EntityFrameworkCore;
using Polaris.WMS.InventoryManage.Domain.inventories;
using Polaris.WMS.InventoryManage.Domain.Containers;
using Polaris.WMS.InventoryManage.Domain.CycleCountOrders;
using Polaris.WMS.InventoryManage.Domain.TransferOrders;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Polaris.WMS.InventoryManage.EntityFrameworkCore;

// 指定连接字符串的名字。默认叫 "Default"，
// 如果未来主数据要用独立数据库，只需要在 appsettings.json 里加一个 "MasterData" 的连接串即可。
[ConnectionStringName("Default")]
public class InventoryDbContext : AbpDbContext<InventoryDbContext>, IInventoryDbContext
{
    // 声明当前模块管辖的实体
    public DbSet<Container> Containers { get; set; }

    // Inventory
    public DbSet<Inventory> Inventorys { get; set; }
    public DbSet<InventoryTransaction> InventoryTransactions { get; set; }

    // Transfer
    public DbSet<TransferOrder> TransferOrders { get; set; }
    public DbSet<TransferOrderDetail> TransferOrderDetails { get; set; }

    // CycleCount
    public DbSet<CycleCountOrder> CycleCountOrders { get; set; }
    public DbSet<CycleCountOrderDetail> CycleCountOrderDetails { get; set; }

    public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureInventory();
    }
}