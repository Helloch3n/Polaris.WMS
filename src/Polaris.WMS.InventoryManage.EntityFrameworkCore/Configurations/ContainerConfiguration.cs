using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Polaris.WMS.InventoryManage.Domain.Containers;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Polaris.WMS.InventoryManage.EntityFrameworkCore.Configurations
{
    public class ReelConfiguration : IEntityTypeConfiguration<Container>
    {
        public void Configure(EntityTypeBuilder<Container> builder)
        {
            builder.ToTable("AppReels");

            builder.ConfigureByConvention();

            builder.Property(x => x.ContainerCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Size)
                .HasMaxLength(100);

            builder.Property(x => x.SelfWeight)
                .HasPrecision(18, 4);

            builder.Property(x => x.Status)
                .HasConversion<int>();

            builder.Property(x => x.IsLocked)
                .HasDefaultValue(false);

            builder.HasIndex(x => x.ContainerCode)
                .IsUnique();
        }
    }
}