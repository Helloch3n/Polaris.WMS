using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Polaris.WMS.MasterData.Domain.AccountAliases;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Polaris.WMS.MasterData.EntityFrameworkCore.Configurations;

public class AccountAliasConfiguration : IEntityTypeConfiguration<AccountAlias>
{
    public void Configure(EntityTypeBuilder<AccountAlias> builder)
    {
        builder.ToTable("AppAccountAliases");

        builder.ConfigureByConvention();

        builder.Property(x => x.Alias)
            .IsRequired()
            .HasMaxLength(100)
            .HasComment("账户别名");

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(500)
            .HasComment("账户别名说明");

        builder.Property(x => x.EffectiveDate)
            .HasColumnType("timestamp without time zone")
            .HasComment("生效日期");

        builder.Property(x => x.ExpireDate)
            .HasColumnType("timestamp without time zone")
            .HasComment("失效日期");

        builder.Property(x => x.IsUnitPriceRequired).HasComment("是否填写单价");
        builder.Property(x => x.IsProjectRequired).HasComment("是否填写项目");
        builder.Property(x => x.IsDepartmentRequired).HasComment("是否填写部门");
        builder.Property(x => x.IsProductionNoRequired).HasComment("是否填写生产号");
        builder.Property(x => x.IsWorkOrderOperationRequired).HasComment("是否填写工单工序");
        builder.Property(x => x.ProductionCostType).HasComment("生产成本类型");
        builder.Property(x => x.IsSupplierRequired).HasComment("是否填写供应商");
        builder.Property(x => x.IsCustomerRequired).HasComment("是否填写客户");
        builder.Property(x => x.IsWorkOrderAttributeRequired).HasComment("是否填写工单属性");

        builder.HasIndex(x => x.Alias).IsUnique();
    }
}

