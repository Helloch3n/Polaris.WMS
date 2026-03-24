using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Polaris.WMS.MasterData.Domain.Locations;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Polaris.WMS.MasterData.EntityFrameworkCore.Configurations
{
    public class LocationConfiguration : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)
        {
            builder.ToTable("AppLocations");

            builder.ConfigureByConvention();

            builder.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(50)
                .HasComment("库位编码");

            builder.Property(x => x.Aisle)
                .HasMaxLength(20)
                .HasComment("巷道");

            builder.Property(x => x.Rack)
                .HasMaxLength(20)
                .HasComment("排");

            builder.Property(x => x.Level)
                .HasMaxLength(20)
                .HasComment("层");

            builder.Property(x => x.Bin)
                .HasMaxLength(20)
                .HasComment("格");

            builder.Property(x => x.MaxWeight)
                .HasPrecision(18, 2)
                .HasComment("最大承重");

            builder.Property(x => x.MaxVolume)
                .HasPrecision(18, 2)
                .HasComment("最大体积");

            builder.Property(x => x.MaxReelCount)
                .HasDefaultValue(1)
                .HasComment("最大盘数");

            builder.Property(x => x.Status)
                .HasComment("库位状态");

            builder.Property(x => x.Type)
                .HasComment("库位类型");

            builder.Property(x => x.AllowMixedProducts)
                .HasDefaultValue(false)
                .HasComment("是否允许混放不同物料");

            builder.Property(x => x.AllowMixedBatches)
                .HasDefaultValue(false)
                .HasComment("是否允许混放不同批次");

            builder.HasIndex(x => new { x.ZoneId, x.Code }).IsUnique();
            builder.HasIndex(x => x.WarehouseId);
            builder.HasIndex(x => x.ZoneId);
        }
    }
}
