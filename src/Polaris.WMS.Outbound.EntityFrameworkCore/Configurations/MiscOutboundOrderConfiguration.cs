using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Polaris.WMS.Outbound.Domain.MiscOrders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Polaris.WMS.Outbound.EntityFrameworkCore.Configurations;

public class MiscOutboundOrderConfiguration : IEntityTypeConfiguration<MiscOutboundOrder>
{
    public void Configure(EntityTypeBuilder<MiscOutboundOrder> builder)
    {
        builder.ConfigureByConvention();
        builder.ToTable("AppMiscOutboundOrders");

        builder.Property(x => x.OrderNo).IsRequired().HasMaxLength(64);
        builder.Property(x => x.AccountAliasDescription).IsRequired().HasMaxLength(500);
        builder.Property(x => x.CostCenterCode).IsRequired().HasMaxLength(50);
        builder.Property(x => x.CostCenterName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Type).HasConversion<int>().IsRequired();
        builder.Property(x => x.Status).HasConversion<int>().IsRequired();
        builder.Property(x => x.Remark).HasMaxLength(1000);

        builder.HasIndex(x => x.OrderNo).IsUnique();
        builder.HasIndex(x => x.AccountAliasId);
        builder.HasIndex(x => x.CostCenterId);

        builder.HasMany(x => x.Details)
            .WithOne()
            .HasForeignKey(x => x.MiscOutboundOrderId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(MiscOutboundOrder.Details))?
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}

