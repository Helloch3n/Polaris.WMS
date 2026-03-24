using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Polaris.WMS.InventoryManage.Domain.Reels;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Polaris.WMS.InventoryManage.EntityFrameworkCore.Configurations
{
    public class ReelConfiguration : IEntityTypeConfiguration<Reel>
    {
        public void Configure(EntityTypeBuilder<Reel> builder)
        {
            builder.ToTable("AppReels");

            builder.ConfigureByConvention();

            builder.Property(x => x.ReelNo)
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

            builder.HasIndex(x => x.ReelNo)
                .IsUnique();
        }
    }
}