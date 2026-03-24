using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Polaris.WMS.MasterData.EntityFrameworkCore.Configurations
{
    public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            builder.ToTable("AppSuppliers");

            builder.ConfigureByConvention();

            builder.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(50)
                .HasComment("供应商编码");

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200)
                .HasComment("供应商名称");

            builder.Property(x => x.ContactPerson)
                .HasMaxLength(50)
                .HasComment("联系人");

            builder.Property(x => x.Mobile)
                .HasMaxLength(30)
                .HasComment("联系电话");

            builder.Property(x => x.Email)
                .HasMaxLength(100)
                .HasComment("邮箱");

            builder.Property(x => x.Address)
                .HasMaxLength(500)
                .HasComment("地址");

            builder.HasIndex(x => x.Code).IsUnique();
        }
    }
}
