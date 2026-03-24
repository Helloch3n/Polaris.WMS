using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Polaris.WMS.Users;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Polaris.WMS.MasterData.EntityFrameworkCore.Configurations
{
    public class UserWarehouseConfiguration : IEntityTypeConfiguration<UserWarehouse>
    {
        public void Configure(EntityTypeBuilder<UserWarehouse> builder)
        {
            builder.ToTable("AppUserWarehouse");
            builder.ConfigureByConvention();

            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.WarehouseId).IsRequired();

            builder.HasIndex(x => new { x.UserId, x.WarehouseId }).IsUnique();
        }
    }
}
