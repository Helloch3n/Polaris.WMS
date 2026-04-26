using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Polaris.WMS.Outbound.Domain.SalesOrders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Polaris.WMS.Outbound.EntityFrameworkCore.Configurations;

public class SalesOrderConfiguration : IEntityTypeConfiguration<SalesOrder>
{
    public void Configure(EntityTypeBuilder<SalesOrder> builder)
    {
        builder.ConfigureByConvention();
        builder.ToTable("AppSalesOrders");

        builder.Property(x => x.OrderNo).IsRequired().HasMaxLength(64);
        builder.Property(x => x.CustomerCode).IsRequired().HasMaxLength(50);
        builder.Property(x => x.CustomerName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Status).HasConversion<int>().IsRequired();
        builder.Property(x => x.Remark).HasMaxLength(1000);

        builder.HasIndex(x => x.OrderNo).IsUnique();
        builder.HasIndex(x => x.CustomerId);
        builder.HasIndex(x => x.Status);

        builder.HasMany(x => x.Details)
            .WithOne()
            .HasForeignKey(x => x.SalesOrderId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(SalesOrder.Details))?
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class SalesOrderDetailConfiguration : IEntityTypeConfiguration<SalesOrderDetail>
{
    public void Configure(EntityTypeBuilder<SalesOrderDetail> builder)
    {
        builder.ConfigureByConvention();
        builder.ToTable("AppSalesOrderDetails");

        builder.Property(x => x.ProductCode).IsRequired().HasMaxLength(50);
        builder.Property(x => x.ProductName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Unit).IsRequired().HasMaxLength(20);
        builder.Property(x => x.Qty).HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.AllocatedQty).HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.ShippedQty).HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.Remark).HasMaxLength(1000);

        builder.HasIndex(x => x.SalesOrderId);
        builder.HasIndex(x => new { x.SalesOrderId, x.LineNo }).IsUnique();
        builder.HasIndex(x => x.ProductId);
    }
}

