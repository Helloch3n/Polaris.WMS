using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Polaris.WMS.InventoryManage.Domain.CycleCountOrders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Polaris.WMS.InventoryManage.EntityFrameworkCore.Configurations;

public class CycleCountOrderDetailConfiguration : IEntityTypeConfiguration<CycleCountOrderDetail>
{
    public void Configure(EntityTypeBuilder<CycleCountOrderDetail> builder)
    {
        builder.ConfigureByConvention();
        builder.ToTable("AppCycleCountOrderDetails");

        builder.Property(x => x.ContainerCode).IsRequired().HasMaxLength(64);
        builder.Property(x => x.SystemQty).HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.CountedQty).HasPrecision(18, 4);
        builder.Property(x => x.DifferenceQty).HasPrecision(18, 4).IsRequired();

        builder.HasIndex(x => x.CycleCountOrderId);
        builder.HasIndex(x => x.LocationId);
        builder.HasIndex(x => new { x.ContainerCode, x.ProductId });
    }
}

