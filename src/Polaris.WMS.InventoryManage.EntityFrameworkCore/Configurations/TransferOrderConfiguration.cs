using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Polaris.WMS.InventoryManage.Domain.TransferOrders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Polaris.WMS.InventoryManage.EntityFrameworkCore.Configurations
{
    public class TransferOrderConfiguration : IEntityTypeConfiguration<TransferOrder>
    {
        public void Configure(EntityTypeBuilder<TransferOrder> builder)
        {
            builder.ToTable("AppTransferOrders");

            builder.ConfigureByConvention();

            builder.Property(x => x.OrderNo)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(x => x.SourceWarehouseId)
                .IsRequired();

            builder.Property(x => x.SourceDepartmentId)
                .IsRequired();

            builder.Property(x => x.TargetWarehouseId)
                .IsRequired();

            builder.HasIndex(x => x.OrderNo).IsUnique();

            builder.HasMany(x => x.Details)
                .WithOne()
                .HasForeignKey(x => x.TransferOrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
