using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Polaris.WMS.MasterData.EntityFrameworkCore.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("AppProducts");

            builder.ConfigureByConvention();

            builder.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(50)
                .HasComment("物料编码");

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200)
                .HasComment("物料名称");

            builder.Property(x => x.Unit)
                .HasMaxLength(20)
                .HasComment("单位");

            builder.Property(x => x.AuxUnit)
                .HasMaxLength(20)
                .HasComment("辅助单位");

            builder.Property(x => x.ConversionRate)
                .HasComment("转换比例");

            builder.Property(x => x.IsBatchManagementEnabled)
                .HasComment("批次管理");

            builder.Property(x => x.ShelfLifeDays)
                .HasComment("保质期(天)");

            builder.HasIndex(x => x.Code).IsUnique();
        }
    }
}
