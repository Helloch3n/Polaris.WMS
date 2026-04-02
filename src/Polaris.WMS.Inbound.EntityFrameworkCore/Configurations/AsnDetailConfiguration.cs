using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Polaris.WMS.Inbound.Domain.Asns;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Polaris.WMS.Inbound.EntityFrameworkCore.Configurations;

public class AsnDetailConfiguration : IEntityTypeConfiguration<AsnDetail>
{
    public void Configure(EntityTypeBuilder<AsnDetail> builder)
    {
        builder.ConfigureByConvention();

        builder.ToTable("AppAsnDetails");

        // 我们决定的 SCM 原始行号（解析自二维码）
        builder.Property(x => x.ScmAsnRowNo).IsRequired().HasMaxLength(64);

        // 溯源 ERP 采购单的字段
        builder.Property(x => x.SourcePoNo).IsRequired().HasMaxLength(64);

        builder.Property(x => x.ProductCode).IsRequired().HasMaxLength(64);
        builder.Property(x => x.ProductName).HasMaxLength(256);
        builder.Property(x => x.UoM).IsRequired().HasMaxLength(16);
        builder.Property(x => x.SupplierBatchNo).HasMaxLength(64);
        builder.Property(x => x.LicensePlate).HasMaxLength(32);

        // 数字精度
        builder.Property(x => x.ExpectedQty).HasColumnType("decimal(18, 4)");
        builder.Property(x => x.ReceivedQty).HasColumnType("decimal(18, 4)");

        // 💡 关键索引 1：在当前 ASN 中，SCM 传来的行号应当唯一，方便 PDA 解析 JSON 后精准定位
        builder.HasIndex(x => new { x.AsnId, x.ScmAsnRowNo }).IsUnique();

        // 💡 关键索引 2：为了加快跨聚合溯源查询 (ASN -> PO)
        builder.HasIndex(x => new { x.SourcePoNo, x.SourcePoLineNo });
    }
}