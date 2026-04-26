using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Polaris.WMS.Outbound.Domain.SalesShipments;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Polaris.WMS.Outbound.EntityFrameworkCore.Configurations;

public class SalesShipmentConfiguration : IEntityTypeConfiguration<SalesShipment>
{
    public void Configure(EntityTypeBuilder<SalesShipment> builder)
    {
        builder.ConfigureByConvention();
        builder.ToTable("AppSalesShipments");

        builder.Property(x => x.ShipmentNo).IsRequired().HasMaxLength(64);
        builder.Property(x => x.SourceSalesOrderNo).HasMaxLength(64);
        builder.Property(x => x.CustomerCode).IsRequired().HasMaxLength(50);
        builder.Property(x => x.CustomerName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.DeliveryContactName).HasMaxLength(50);
        builder.Property(x => x.DeliveryPhone).HasMaxLength(30);
        builder.Property(x => x.DeliveryAddress).HasMaxLength(500);
        builder.Property(x => x.Status).HasConversion<int>().IsRequired();
        builder.Property(x => x.Remark).HasMaxLength(1000);

        builder.HasIndex(x => x.ShipmentNo).IsUnique();
        builder.HasIndex(x => x.SourceSalesOrderId);
        builder.HasIndex(x => x.CustomerId);
        builder.HasIndex(x => x.Status);

        builder.HasMany(x => x.Details)
            .WithOne()
            .HasForeignKey(x => x.SalesShipmentId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(SalesShipment.Details))?
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class SalesShipmentDetailConfiguration : IEntityTypeConfiguration<SalesShipmentDetail>
{
    public void Configure(EntityTypeBuilder<SalesShipmentDetail> builder)
    {
        builder.ConfigureByConvention();
        builder.ToTable("AppSalesShipmentDetails");

        builder.Property(x => x.ProductCode).IsRequired().HasMaxLength(50);
        builder.Property(x => x.ProductName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Unit).IsRequired().HasMaxLength(20);
        builder.Property(x => x.RequiredQty).HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.AllocatedQty).HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.PickedQty).HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.ShippedQty).HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.Remark).HasMaxLength(1000);

        builder.HasIndex(x => x.SalesShipmentId);
        builder.HasIndex(x => new { x.SalesShipmentId, x.LineNo }).IsUnique();
        builder.HasIndex(x => x.SourceSalesOrderLineId);
        builder.HasIndex(x => x.ProductId);

        builder.HasMany(x => x.Records)
            .WithOne()
            .HasForeignKey(x => x.SalesShipmentDetailId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(SalesShipmentDetail.Records))?
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class SalesShipmentAllocationConfiguration : IEntityTypeConfiguration<SalesShipmentAllocation>
{
    public void Configure(EntityTypeBuilder<SalesShipmentAllocation> builder)
    {
        builder.ConfigureByConvention();
        builder.ToTable("AppSalesShipmentAllocations");

        builder.Property(x => x.ProductCode).IsRequired().HasMaxLength(50);
        builder.Property(x => x.ProductName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Qty).HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.ContainerCode).IsRequired().HasMaxLength(64);
        builder.Property(x => x.SourceLocationCode).IsRequired().HasMaxLength(64);
        builder.Property(x => x.BatchNo).IsRequired().HasMaxLength(100);
        builder.Property(x => x.SN).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Status).HasConversion<int>().IsRequired();

        builder.HasIndex(x => x.SalesShipmentId);
        builder.HasIndex(x => x.SalesShipmentDetailId);
        builder.HasIndex(x => x.ProductId);
        builder.HasIndex(x => x.ContainerId);
        builder.HasIndex(x => x.SourceLocationId);
        builder.HasIndex(x => x.Status);
    }
}

public class SalesShipmentRecordConfiguration : IEntityTypeConfiguration<SalesShipmentRecord>
{
    public void Configure(EntityTypeBuilder<SalesShipmentRecord> builder)
    {
        builder.ConfigureByConvention();
        builder.ToTable("AppSalesShipmentRecords");

        builder.Property(x => x.ProductCode).IsRequired().HasMaxLength(50);
        builder.Property(x => x.ProductName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Qty).HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.ContainerCode).IsRequired().HasMaxLength(64);
        builder.Property(x => x.LocationCode).IsRequired().HasMaxLength(64);
        builder.Property(x => x.BatchNo).IsRequired().HasMaxLength(100);
        builder.Property(x => x.SN).IsRequired().HasMaxLength(100);

        builder.HasIndex(x => x.SalesShipmentId);
        builder.HasIndex(x => x.SalesShipmentDetailId);
        builder.HasIndex(x => x.SourceSalesOrderLineId);
        builder.HasIndex(x => x.ContainerId);
        builder.HasIndex(x => x.LocationId);
        builder.HasIndex(x => x.ProductId);
    }
}


