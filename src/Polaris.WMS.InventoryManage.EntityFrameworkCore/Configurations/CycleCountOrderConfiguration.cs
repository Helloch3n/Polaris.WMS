using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Polaris.WMS.InventoryManage.Domain.CycleCountOrders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Polaris.WMS.InventoryManage.EntityFrameworkCore.Configurations;

public class CycleCountOrderConfiguration : IEntityTypeConfiguration<CycleCountOrder>
{
    public void Configure(EntityTypeBuilder<CycleCountOrder> builder)
    {
        builder.ConfigureByConvention();
        builder.ToTable("AppCycleCountOrders");

        builder.Property(x => x.OrderNo).IsRequired().HasMaxLength(64);
        builder.Property(x => x.CountType).HasConversion<int>().IsRequired();
        builder.Property(x => x.Status).HasConversion<int>().IsRequired();

        builder.HasIndex(x => x.OrderNo).IsUnique();
        builder.HasIndex(x => x.WarehouseId);

        builder.HasMany(x => x.Details)
            .WithOne()
            .HasForeignKey(x => x.CycleCountOrderId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(CycleCountOrder.Details))?
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}

