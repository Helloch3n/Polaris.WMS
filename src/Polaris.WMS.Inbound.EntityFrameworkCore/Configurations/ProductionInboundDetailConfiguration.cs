using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Polaris.WMS.Inbound.Domain.ProductionInbounds;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Polaris.WMS.Inbound.EntityFrameworkCore.Configurations
{
    public class ProductionInboundDetailConfiguration : IEntityTypeConfiguration<ProductionInboundDetail>
    {
        public void Configure(EntityTypeBuilder<ProductionInboundDetail> builder)
        {
            builder.ToTable("AppProductionInboundDetails");

            // 基础配置
            builder.ConfigureByConvention();

            // 字段长度限制
            builder.Property(x => x.BatchNo).HasMaxLength(64);
            builder.Property(x => x.Unit).IsRequired().HasMaxLength(16);
            builder.Property(x => x.SN).HasMaxLength(128);
            builder.Property(x => x.CraftVersion).HasMaxLength(64);
            builder.Property(x => x.RelatedOrderNo).HasMaxLength(64);
            builder.Property(x => x.RelatedOrderNoLineNo).HasMaxLength(64);

            // Decimal 精度
            builder.Property(x => x.Qty).HasPrecision(18, 4);
            builder.Property(x => x.Weight).HasPrecision(18, 4);

            // 状态
            builder.Property(x => x.Status).HasConversion<int>().IsRequired();

            // 索引配置
            builder.HasIndex(x => x.ProductionInboundId); // 外键索引
            builder.HasIndex(x => x.ReelId);              // 盘具关联索引
            builder.HasIndex(x => x.BatchNo);             // 批次号查询索引
            builder.HasIndex(x => x.SN);                  // 唯一码查询索引
        }
    }
}
