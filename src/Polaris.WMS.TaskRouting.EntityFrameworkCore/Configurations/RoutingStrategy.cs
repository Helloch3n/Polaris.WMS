using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Polaris.WMS.TaskRouting.Domain.LogisticsStrategies;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Polaris.WMS.TaskRouting.EntityFrameworkCore.Configurations
{
    public class RoutingStrategyConfiguration : IEntityTypeConfiguration<RoutingStrategy>
    {
        public void Configure(EntityTypeBuilder<RoutingStrategy> builder)
        {
            // Table & conventions
            builder.ToTable("AppRoutingStrategies");

            builder.ConfigureByConvention();

            // 基本字段
            builder.Property(rs => rs.RuleName)
                .IsRequired()
                .HasMaxLength(200)
                .HasComment("路由规则名称");

            builder.Property(rs => rs.Priority)
                .IsRequired()
                .HasComment("规则优先级，数字越小越优先");

            // Enum -> int 存储
            builder.Property(rs => rs.TaskType)
                .HasConversion<int>()
                .IsRequired()
                .HasComment("任务类型");

            builder.Property(rs => rs.IsActive)
                .IsRequired()
                .HasDefaultValue(true)
                .HasComment("是否启用");

            // 匹配条件（可为空）
            builder.Property(rs => rs.SourceZoneId)
                .IsRequired(false)
                .HasComment("来源分区/库区 Id（可选）");

            builder.Property(rs => rs.ProductCategoryId)
                .IsRequired(false)
                .HasComment("物料类别 Id（可选）");

            builder.Property(rs => rs.ProductId)
                .IsRequired(false)
                .HasComment("物料 Id（可选）");

            // 路由结果
            builder.Property(rs => rs.TargetZoneId)
                .IsRequired()
                .HasComment("目标分区/库区 Id");

            // 索引：常用的查询组合
            builder.HasIndex(rs => rs.RuleName);
            builder.HasIndex(rs => new { rs.TaskType, rs.IsActive, rs.Priority });
        }
    }
}