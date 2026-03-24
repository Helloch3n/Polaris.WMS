using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Polaris.WMS.Inbound.Domain.ProductionInbounds;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Polaris.WMS.Inbound.EntityFrameworkCore.Configurations
{
    public class ProductionInboundConfiguration : IEntityTypeConfiguration<ProductionInbound>
    {
        public void Configure(EntityTypeBuilder<ProductionInbound> builder)
        {
            // 1. 设置表名 (推荐加上你们的前缀，比如 WmsProductionInbounds)
            builder.ToTable("AppProductionInbounds");

            // 2. 基础配置：让 ABP 自动映射基础字段 (Id, CreationTime, CreatorId, IsDeleted 等软删除和审计字段)
            builder.ConfigureByConvention();

            // 3. 字段属性配置
            // 入库单号：必须且限制长度
            builder.Property(x => x.OrderNo).IsRequired().HasMaxLength(64);
            // 来源单号
            builder.Property(x => x.SourceOrderNo).HasMaxLength(64);

            // 4. 索引配置
            // WMS 内部单号必须全局唯一
            builder.HasIndex(x => x.OrderNo).IsUnique();
            // 来源单号经常用于查询，加上普通索引
            builder.HasIndex(x => x.SourceOrderNo);
            // 为隔离字段加索引，极大提升查询列表的速度
            builder.HasIndex(x => x.WarehouseId);
            builder.HasIndex(x => x.DepartmentId);

            // 5. 🌟 核心：DDD 聚合根一对多关系映射
            // 告诉 EF Core：一个头表拥有多个 Details
            builder.HasMany(x => x.Details)
                   .WithOne() // 因为 Detail 实体中没有指向主表的引用属性，只留空即可
                   .HasForeignKey(x => x.ProductionInboundId) // 指定外键
                   .IsRequired() // 外键必填
                   .OnDelete(DeleteBehavior.Cascade); // 级联删除：如果删除了(或软删除)主表，明细一起删
        }
    }
}