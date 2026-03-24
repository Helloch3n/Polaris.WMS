using Microsoft.EntityFrameworkCore;
using Volo.Abp;

namespace Polaris.WMS.Outbound.EntityFrameworkCore;

public static class OutboundDbContextModelBuilderExtensions
{
    public static void ConfigureOutBound(this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        // 让 EF Core 自动扫描当前程序集里的所有 IEntityTypeConfiguration<T>
        builder.ApplyConfigurationsFromAssembly(typeof(OutboundDbContextModelBuilderExtensions).Assembly);
    }
}