using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Polaris.WMS.Outbound.Domain.Handovers;
using Polaris.WMS.Outbound.Domain.PickLists;
using Polaris.WMS.Outbound.Domain.Reviews;
using Polaris.WMS.Outbound.Domain.WaveOrders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Polaris.WMS.Outbound.EntityFrameworkCore.Configurations;

public class WaveOrderConfiguration : IEntityTypeConfiguration<WaveOrder>
{
    public void Configure(EntityTypeBuilder<WaveOrder> builder)
    {
        builder.ConfigureByConvention();
        builder.ToTable("AppWaveOrders");

        builder.Property(x => x.WaveNo).IsRequired().HasMaxLength(64);
        builder.Property(x => x.Status).HasConversion<int>().IsRequired();
        builder.Property(x => x.Remark).HasMaxLength(1000);

        builder.HasIndex(x => x.WaveNo).IsUnique();
        builder.HasIndex(x => x.Status);

        builder.HasMany(x => x.Lines)
            .WithOne()
            .HasForeignKey(x => x.WaveOrderId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(WaveOrder.Lines))?
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class WaveOrderLineConfiguration : IEntityTypeConfiguration<WaveOrderLine>
{
    public void Configure(EntityTypeBuilder<WaveOrderLine> builder)
    {
        builder.ConfigureByConvention();
        builder.ToTable("AppWaveOrderLines");

        builder.Property(x => x.SalesShipmentNo).IsRequired().HasMaxLength(64);
        builder.Property(x => x.CustomerCode).IsRequired().HasMaxLength(50);
        builder.Property(x => x.CustomerName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.ProductCode).IsRequired().HasMaxLength(50);
        builder.Property(x => x.ProductName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Qty).HasPrecision(18, 4).IsRequired();

        builder.HasIndex(x => x.WaveOrderId);
        builder.HasIndex(x => x.SalesShipmentId);
        builder.HasIndex(x => x.SalesShipmentDetailId);
        builder.HasIndex(x => x.ProductId);
    }
}

public class PickListConfiguration : IEntityTypeConfiguration<PickList>
{
    public void Configure(EntityTypeBuilder<PickList> builder)
    {
        builder.ConfigureByConvention();
        builder.ToTable("AppPickLists");

        builder.Property(x => x.PickNo).IsRequired().HasMaxLength(64);
        builder.Property(x => x.TargetLocationCode).IsRequired().HasMaxLength(64);
        builder.Property(x => x.Status).HasConversion<int>().IsRequired();
        builder.Property(x => x.Remark).HasMaxLength(1000);

        builder.HasIndex(x => x.PickNo).IsUnique();
        builder.HasIndex(x => x.WaveOrderId);
        builder.HasIndex(x => x.TargetLocationId);
        builder.HasIndex(x => x.Status);

        builder.HasMany(x => x.Lines)
            .WithOne()
            .HasForeignKey(x => x.PickListId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(PickList.Lines))?
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class PickListLineConfiguration : IEntityTypeConfiguration<PickListLine>
{
    public void Configure(EntityTypeBuilder<PickListLine> builder)
    {
        builder.ConfigureByConvention();
        builder.ToTable("AppPickListLines");

        builder.Property(x => x.SalesShipmentNo).IsRequired().HasMaxLength(64);
        builder.Property(x => x.ProductCode).IsRequired().HasMaxLength(50);
        builder.Property(x => x.ProductName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Qty).HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.ContainerCode).IsRequired().HasMaxLength(64);
        builder.Property(x => x.SourceLocationCode).IsRequired().HasMaxLength(64);
        builder.Property(x => x.TargetLocationCode).IsRequired().HasMaxLength(64);
        builder.Property(x => x.MoveTaskNo).HasMaxLength(64);
        builder.Property(x => x.BatchNo).IsRequired().HasMaxLength(100);
        builder.Property(x => x.SN).IsRequired().HasMaxLength(100);

        builder.HasIndex(x => x.PickListId);
        builder.HasIndex(x => x.SalesShipmentId);
        builder.HasIndex(x => x.SalesShipmentDetailId);
        builder.HasIndex(x => x.SalesShipmentAllocationId);
        builder.HasIndex(x => x.ProductId);
        builder.HasIndex(x => x.ContainerId);
        builder.HasIndex(x => x.SourceLocationId);
        builder.HasIndex(x => x.TargetLocationId);
        builder.HasIndex(x => x.MoveTaskId).IsUnique(false);
    }
}

public class OutboundReviewOrderConfiguration : IEntityTypeConfiguration<OutboundReviewOrder>
{
    public void Configure(EntityTypeBuilder<OutboundReviewOrder> builder)
    {
        builder.ConfigureByConvention();
        builder.ToTable("AppOutboundReviewOrders");

        builder.Property(x => x.ReviewNo).IsRequired().HasMaxLength(64);
        builder.Property(x => x.Status).HasConversion<int>().IsRequired();
        builder.Property(x => x.Remark).HasMaxLength(1000);

        builder.HasIndex(x => x.ReviewNo).IsUnique();
        builder.HasIndex(x => x.PickListId);
        builder.HasIndex(x => x.Status);

        builder.HasMany(x => x.Lines)
            .WithOne()
            .HasForeignKey(x => x.OutboundReviewOrderId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(OutboundReviewOrder.Lines))?
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class OutboundReviewLineConfiguration : IEntityTypeConfiguration<OutboundReviewLine>
{
    public void Configure(EntityTypeBuilder<OutboundReviewLine> builder)
    {
        builder.ConfigureByConvention();
        builder.ToTable("AppOutboundReviewLines");

        builder.Property(x => x.ProductCode).IsRequired().HasMaxLength(50);
        builder.Property(x => x.ProductName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Qty).HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.ContainerCode).IsRequired().HasMaxLength(64);
        builder.Property(x => x.LocationCode).IsRequired().HasMaxLength(64);
        builder.Property(x => x.BatchNo).IsRequired().HasMaxLength(100);
        builder.Property(x => x.SN).IsRequired().HasMaxLength(100);

        builder.HasIndex(x => x.OutboundReviewOrderId);
        builder.HasIndex(x => x.PickListLineId);
        builder.HasIndex(x => x.SalesShipmentId);
        builder.HasIndex(x => x.SalesShipmentDetailId);
        builder.HasIndex(x => x.SalesShipmentAllocationId);
    }
}

public class OutboundHandoverOrderConfiguration : IEntityTypeConfiguration<OutboundHandoverOrder>
{
    public void Configure(EntityTypeBuilder<OutboundHandoverOrder> builder)
    {
        builder.ConfigureByConvention();
        builder.ToTable("AppOutboundHandoverOrders");

        builder.Property(x => x.HandoverNo).IsRequired().HasMaxLength(64);
        builder.Property(x => x.Status).HasConversion<int>().IsRequired();
        builder.Property(x => x.Remark).HasMaxLength(1000);

        builder.HasIndex(x => x.HandoverNo).IsUnique();
        builder.HasIndex(x => x.OutboundReviewOrderId);
        builder.HasIndex(x => x.Status);

        builder.HasMany(x => x.Lines)
            .WithOne()
            .HasForeignKey(x => x.OutboundHandoverOrderId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(OutboundHandoverOrder.Lines))?
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class OutboundHandoverLineConfiguration : IEntityTypeConfiguration<OutboundHandoverLine>
{
    public void Configure(EntityTypeBuilder<OutboundHandoverLine> builder)
    {
        builder.ConfigureByConvention();
        builder.ToTable("AppOutboundHandoverLines");

        builder.Property(x => x.ProductCode).IsRequired().HasMaxLength(50);
        builder.Property(x => x.ProductName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Qty).HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.ContainerCode).IsRequired().HasMaxLength(64);
        builder.Property(x => x.LocationCode).IsRequired().HasMaxLength(64);
        builder.Property(x => x.BatchNo).IsRequired().HasMaxLength(100);
        builder.Property(x => x.SN).IsRequired().HasMaxLength(100);

        builder.HasIndex(x => x.OutboundHandoverOrderId);
        builder.HasIndex(x => x.ReviewLineId);
        builder.HasIndex(x => x.SalesShipmentId);
        builder.HasIndex(x => x.SalesShipmentDetailId);
        builder.HasIndex(x => x.SalesShipmentAllocationId);
    }
}

