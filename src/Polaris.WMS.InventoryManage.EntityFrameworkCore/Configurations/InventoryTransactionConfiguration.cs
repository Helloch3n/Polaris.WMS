using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Polaris.WMS.InventoryManage.Domain.inventories;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Polaris.WMS.InventoryManage.EntityFrameworkCore.Configurations
{
    public class InventoryTransactionConfiguration : IEntityTypeConfiguration<InventoryTransaction>
    {
        public void Configure(EntityTypeBuilder<InventoryTransaction> builder)
        {
            builder.ToTable("AppInventoryTransactions");

            builder.ConfigureByConvention();

            builder.HasIndex(x => x.BillNo);
            builder.HasIndex(x => x.InventoryId);
            builder.HasIndex(x => x.CreationTime);
        }
    }
}