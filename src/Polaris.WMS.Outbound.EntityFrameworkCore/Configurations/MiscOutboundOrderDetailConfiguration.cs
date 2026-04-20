using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Polaris.WMS.Outbound.Domain.MiscOrders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Polaris.WMS.Outbound.EntityFrameworkCore.Configurations;

public class MiscOutboundOrderDetailConfiguration : IEntityTypeConfiguration<MiscOutboundOrderDetail>
{
    public void Configure(EntityTypeBuilder<MiscOutboundOrderDetail> builder)
    {
        builder.ConfigureByConvention();
        builder.ToTable("AppMiscOutboundOrderDetails");

        builder.Property(x => x.WarehouseCode).IsRequired().HasMaxLength(64);
        builder.Property(x => x.WarehouseName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.LocationCode).IsRequired().HasMaxLength(64);
        builder.Property(x => x.ContainerCode).IsRequired().HasMaxLength(64);
        builder.Property(x => x.ProductCode).IsRequired().HasMaxLength(50);
        builder.Property(x => x.ProductName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.SN).IsRequired().HasMaxLength(100);
        builder.Property(x => x.BatchNo).IsRequired().HasMaxLength(100);
        builder.Property(x => x.CraftVersion).HasMaxLength(50);
        builder.Property(x => x.Unit).IsRequired().HasMaxLength(20);
        builder.Property(x => x.Qty).HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.Remark).HasMaxLength(1000);

        builder.HasIndex(x => x.MiscOutboundOrderId);
        builder.HasIndex(x => x.WarehouseId);
        builder.HasIndex(x => x.LocationId);
        builder.HasIndex(x => x.ContainerId);
        builder.HasIndex(x => x.ProductId);
        builder.HasIndex(x => x.SN);
    }
}

