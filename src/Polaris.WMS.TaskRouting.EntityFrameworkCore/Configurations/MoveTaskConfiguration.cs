using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Polaris.WMS.TaskRouting.Domain.MoveTasks;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Polaris.WMS.TaskRouting.EntityFrameworkCore.Configurations
{
    public class MoveTaskConfiguration : IEntityTypeConfiguration<MoveTask>
    {
        public void Configure(EntityTypeBuilder<MoveTask> builder)
        {
            builder.ToTable("AppMoveTasks");

            builder.ConfigureByConvention();

            builder.Property(x => x.TaskNo)
                .IsRequired()
                .HasMaxLength(64)
                .HasComment("任务编号");

            builder.Property(x => x.TaskType)
                .HasConversion<int>()
                .HasComment("任务类型（枚举）");

            builder.Property(x => x.Status)
                .HasConversion<int>()
                .HasComment("任务状态（枚举）");

            builder.Property(x => x.ContainerId)
                .IsRequired()
                .HasComment("绑定的物理载具Id");

            builder.Property(x => x.SourceLocationId)
                .HasComment("源库位Id");

            builder.Property(x => x.TargetLocationId)
                .IsRequired()
                .HasComment("计划目标库位Id");

            builder.Property(x => x.ActualLocationId)
                .HasComment("实际落位库位Id（完成时由PDA扫码记录）");

            builder.HasIndex(x => x.TaskNo)
                .IsUnique(false);

            builder.HasIndex(x => x.ContainerId);
        }
    }
}

