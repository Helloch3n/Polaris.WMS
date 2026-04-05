using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Polaris.WMS.Inbound.Domain.PurchaseReceipts;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Polaris.WMS.Inbound.EntityFrameworkCore.Configurations;

public class PurchaseReceiptDetailConfiguration : IEntityTypeConfiguration<PurchaseReceiptDetail>
{
    public void Configure(EntityTypeBuilder<PurchaseReceiptDetail> builder)
    {
        builder.ConfigureByConvention();
        builder.ToTable("AppPurchaseReceiptDetails");

        builder.Property(x => x.ProductCode).IsRequired().HasMaxLength(64);
        builder.Property(x => x.ProductName).IsRequired().HasMaxLength(256);
        builder.Property(x => x.ContainerCode).IsRequired().HasMaxLength(64);
        builder.Property(x => x.LocationCode).IsRequired().HasMaxLength(64);
        builder.Property(x => x.BatchNo).HasMaxLength(64);
        builder.Property(x => x.ErpSyncErrorMessage).HasMaxLength(1000);

        builder.Property(x => x.ReceivedQuantity).HasPrecision(18, 4);
        builder.Property(x => x.ErpSyncStatus).HasConversion<int>().IsRequired();

        builder.HasIndex(x => x.PurchaseReceiptId);
        builder.HasIndex(x => x.ProductId);
        builder.HasIndex(x => x.ContainerId);
        builder.HasIndex(x => x.LocationId);
        builder.HasIndex(x => x.ErpSyncStatus);
    }
}

