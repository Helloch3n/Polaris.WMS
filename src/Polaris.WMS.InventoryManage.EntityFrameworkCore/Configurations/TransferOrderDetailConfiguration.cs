using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Polaris.WMS.InventoryManage.Domain.TransferOrders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Polaris.WMS.InventoryManage.EntityFrameworkCore.Configurations
{
    public class TransferOrderDetailConfiguration : IEntityTypeConfiguration<TransferOrderDetail>
    {
        public void Configure(EntityTypeBuilder<TransferOrderDetail> builder)
        {
            builder.ToTable("AppTransferOrderDetails");

            builder.ConfigureByConvention();

            builder.Property(x => x.Qty)
                .HasPrecision(18, 4)
                .IsRequired();

            builder.HasIndex(x => x.TransferOrderId);
            builder.HasIndex(x => x.ContainerId);
            builder.HasIndex(x => x.InventoryId);
        }
    }
}
