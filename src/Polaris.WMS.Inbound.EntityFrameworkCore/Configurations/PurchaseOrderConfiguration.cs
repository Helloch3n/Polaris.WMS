using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Polaris.WMS.Inbound.Domain.PurchaseOrders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Polaris.WMS.Inbound.EntityFrameworkCore.Configurations;

public class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        // ABP 约定配置（自动配置 Id, 审计字段, 租户等）
        builder.ConfigureByConvention();

        builder.ToTable("AppPurchaseOrders");

        // 核心属性配置
        builder.Property(x => x.PoNo).IsRequired().HasMaxLength(64);
        builder.Property(x => x.SupplierCode).IsRequired().HasMaxLength(64);
        builder.Property(x => x.SupplierName).HasMaxLength(256);

        // 索引设计：按 PO 单号查询极其频繁
        builder.HasIndex(x => x.PoNo).IsUnique();
        builder.HasIndex(x => x.SupplierId);

        // 导航属性与外键关系
        builder.HasMany(x => x.Details)
            .WithOne() // 即使 Detail 没有写导航回 PO 的属性，这里也可以留空
            .HasForeignKey(x => x.PurchaseOrderId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade); // 级联删除：删主表带走明细

        // 💡 核心映射：告诉 EF Core 使用私有字段 _details 来给只读集合赋值
        builder.Metadata.FindNavigation(nameof(PurchaseOrder.Details))?
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}