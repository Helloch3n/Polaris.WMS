using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Polaris.WMS.Inbound.Domain.PurchaseOrders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Polaris.WMS.Inbound.EntityFrameworkCore.Configurations;

public class PurchaseOrderDetailConfiguration : IEntityTypeConfiguration<PurchaseOrderDetail>
{
    public void Configure(EntityTypeBuilder<PurchaseOrderDetail> builder)
    {
        builder.ConfigureByConvention();

        builder.ToTable("AppPurchaseOrderDetails");

        // 属性长度限制
        builder.Property(x => x.ProductCode).IsRequired().HasMaxLength(64);
        builder.Property(x => x.ProductName).HasMaxLength(256);
        builder.Property(x => x.UoM).IsRequired().HasMaxLength(16);

        // 💡 高精度数字映射：保留 4 位小数，支撑电缆行业的克重或米数
        builder.Property(x => x.ExpectedQty).HasColumnType("decimal(18, 4)");
        builder.Property(x => x.ReceivedQty).HasColumnType("decimal(18, 4)");
        builder.Property(x => x.DeliveredQty).HasColumnType("decimal(18, 4)");

        // 复合索引：在同一个采购单下，行号必须是唯一的
        builder.HasIndex(x => new { x.PurchaseOrderId, x.LineNo }).IsUnique();
    }
}