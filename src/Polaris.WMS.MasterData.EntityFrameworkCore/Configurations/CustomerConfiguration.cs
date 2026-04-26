using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Polaris.WMS.MasterData.Domain.Customers;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Polaris.WMS.MasterData.EntityFrameworkCore.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("AppCustomers");

        builder.ConfigureByConvention();

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(50)
            .HasComment("客户编码");

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200)
            .HasComment("客户名称");

        builder.Property(x => x.ContactName)
            .HasMaxLength(50)
            .HasComment("联系人");

        builder.Property(x => x.Phone)
            .HasMaxLength(30)
            .HasComment("联系电话");

        builder.Property(x => x.Address)
            .HasMaxLength(500)
            .HasComment("地址");

        builder.Property(x => x.IsEnabled)
            .IsRequired()
            .HasComment("是否启用");

        builder.Property(x => x.Remark)
            .HasMaxLength(1000)
            .HasComment("备注");

        builder.HasIndex(x => x.Code).IsUnique();
    }
}

