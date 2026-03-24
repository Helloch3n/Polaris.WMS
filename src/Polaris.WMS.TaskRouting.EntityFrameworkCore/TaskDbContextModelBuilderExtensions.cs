using Microsoft.EntityFrameworkCore;
using Volo.Abp;

namespace Polaris.WMS.TaskRouting.EntityFrameworkCore;

public static class TaskDbContextModelBuilderExtensions
{
    public static void ConfigureTaskRouting(this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        // 让 EF Core 自动扫描当前程序集里的所有 IEntityTypeConfiguration<T>
        builder.ApplyConfigurationsFromAssembly(typeof(TaskDbContextModelBuilderExtensions).Assembly);
    }
}