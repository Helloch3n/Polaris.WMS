using Microsoft.EntityFrameworkCore;
using Volo.Abp;

namespace Polaris.WMS.InventoryManage.EntityFrameworkCore;

public static class InventoryDbContextModelBuilderExtensions
{
    public static void ConfigureInventory(this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        // 让 EF Core 自动扫描当前程序集里的所有 IEntityTypeConfiguration<T>
        builder.ApplyConfigurationsFromAssembly(typeof(InventoryDbContextModelBuilderExtensions).Assembly);
    }
}