using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Polaris.WMS.Inbound.Domain.Asns;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Polaris.WMS.Inbound.EntityFrameworkCore.Configurations;

public class AdvancedShippingNoticeConfiguration : IEntityTypeConfiguration<AdvancedShippingNotice>
{
    public void Configure(EntityTypeBuilder<AdvancedShippingNotice> builder)
    {
        builder.ConfigureByConvention();

        builder.ToTable("AppAdvancedShippingNotices");

        builder.Property(x => x.AsnNo).IsRequired().HasMaxLength(64);
        builder.Property(x => x.SupplierCode).IsRequired().HasMaxLength(64);
        builder.Property(x => x.SupplierName).HasMaxLength(256);

        builder.HasIndex(x => x.AsnNo).IsUnique();
        builder.HasIndex(x => x.SupplierId);

        // 聚合内明细的级联关系
        builder.HasMany(x => x.Details)
            .WithOne()
            .HasForeignKey(x => x.AsnId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // 幕后字段映射
        builder.Metadata.FindNavigation(nameof(AdvancedShippingNotice.Details))?
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}