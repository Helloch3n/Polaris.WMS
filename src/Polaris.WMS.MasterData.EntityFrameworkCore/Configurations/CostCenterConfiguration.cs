using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Polaris.WMS.MasterData.Domain.CostCenters;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Polaris.WMS.MasterData.EntityFrameworkCore.Configurations;

public class CostCenterConfiguration : IEntityTypeConfiguration<CostCenter>
{
    public void Configure(EntityTypeBuilder<CostCenter> builder)
    {
        builder.ToTable("AppCostCenters");

        builder.ConfigureByConvention();

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(50)
            .HasComment("成本中心编码");

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200)
            .HasComment("成本中心名称");

        builder.Property(x => x.DepartmentCode)
            .IsRequired()
            .HasMaxLength(50)
            .HasComment("部门编码");

        builder.Property(x => x.DepartmentName)
            .IsRequired()
            .HasMaxLength(200)
            .HasComment("部门名称");

        builder.Property(x => x.CompanyCode)
            .IsRequired()
            .HasMaxLength(50)
            .HasComment("公司编码");

        builder.HasIndex(x => x.Code).IsUnique();
    }
}

