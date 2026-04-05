using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Polaris.WMS.Inbound.Domain.PurchaseReceipts;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Polaris.WMS.Inbound.EntityFrameworkCore.Configurations;

public class PurchaseReceiptConfiguration : IEntityTypeConfiguration<PurchaseReceipt>
{
    public void Configure(EntityTypeBuilder<PurchaseReceipt> builder)
    {
        builder.ConfigureByConvention();
        builder.ToTable("AppPurchaseReceipts");

        builder.Property(x => x.ReceiptNo).IsRequired().HasMaxLength(64);
        builder.Property(x => x.SourceDocType).IsRequired().HasMaxLength(32);
        builder.Property(x => x.SourceDocNo).IsRequired().HasMaxLength(64);
        builder.Property(x => x.SupplierName).HasMaxLength(256);
        builder.Property(x => x.Remark).HasMaxLength(1000);

        builder.HasIndex(x => x.ReceiptNo).IsUnique();
        builder.HasIndex(x => new { x.SourceDocType, x.SourceDocNo });
        builder.HasIndex(x => x.SupplierId);

        builder.HasMany(x => x.Details)
            .WithOne()
            .HasForeignKey(x => x.PurchaseReceiptId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(PurchaseReceipt.Details))?
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}

