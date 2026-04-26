using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders; // ABP 10 需要的构建器命名空间
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polaris.WMS.DataSync;
using Polaris.WMS.Inbound;
using Polaris.WMS.Isolation;
using Polaris.WMS.MasterData;
using Polaris.WMS.Outbound;
using Polaris.WMS.ProductionInbounds;
using Polaris.WMS.Putaway;
using Polaris.WMS.TransferOrders;
using Polaris.WMS.Users;
using System;
using System.Linq;
using System.Linq.Expressions;
using Polaris.WMS.Inbound.Domain.Asns;
using Polaris.WMS.Inbound.Domain.MiscOrders;
using Polaris.WMS.Inbound.Domain.ProductionInbounds;
using Polaris.WMS.Inbound.Domain.PurchaseOrders;
using Polaris.WMS.Inbound.Domain.PurchaseReceipts;
using Polaris.WMS.Inbound.EntityFrameworkCore;
using Polaris.WMS.InventoryManage.Domain.Containers;
using Polaris.WMS.InventoryManage.Domain.CycleCountOrders;
using Polaris.WMS.InventoryManage.Domain.inventories;
using Polaris.WMS.InventoryManage.Domain.TransferOrders;
using Polaris.WMS.InventoryManage.EntityFrameworkCore;
using Polaris.WMS.MasterData.Domain.Locations;
using Polaris.WMS.MasterData.Domain.AccountAliases;
using Polaris.WMS.MasterData.Domain.CostCenters;
using Polaris.WMS.MasterData.Domain.warehouses;
using Polaris.WMS.MasterData.Domain.Zones;
using Polaris.WMS.MasterData.EntityFrameworkCore;
using Polaris.WMS.Outbound.Domain.Handovers;
using Polaris.WMS.Outbound.Domain.MiscOrders;
using Polaris.WMS.Outbound.Domain.PickLists;
using Polaris.WMS.Outbound.Domain.Reviews;
using Polaris.WMS.Outbound.Domain.SalesOrders;
using Polaris.WMS.Outbound.Domain.SalesShipments;
using Polaris.WMS.Outbound.Domain.WaveOrders;
using Polaris.WMS.Outbound.EntityFrameworkCore;
using Polaris.WMS.TaskRouting.Domain.LogisticsStrategies;
using Polaris.WMS.TaskRouting.Domain.MoveTasks;
using Polaris.WMS.TaskRouting.EntityFrameworkCore;
using Polaris.WMS.Tasks;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace Polaris.WMS.EntityFrameworkCore;

/// <summary>
/// WMS 的 EF Core 上下文（聚合模块的统一 DbContext）。
///
/// 说明：
/// - 该上下文整合了多个模块的 DbSet，用于 Migrations 与运行时访问。
/// - 通过 <see cref="IWMSContextProvider"/> 提供的上下文信息（当前仓库/部门），在全局查询过滤中引入隔离条件。
/// - 请仅在此文件中维护 DbSet 声明与全局模型配置，具体实体映射请使用独立的 IEntityTypeConfiguration<T> 实现类。
/// </summary>
[ConnectionStringName("Default")]
public class WMSDbContext :
    AbpDbContext<WMSDbContext>,
    ITenantManagementDbContext,
    IIdentityDbContext,
    IMasterDataDbContext,
    ITaskDbContext,
    IInventoryDbContext,
    IInboundDbContext,
    IOutBoundDbContext
{
    // 注：通过 IWMSContextProvider 获取当前请求上下文（仓库/部门），用于拼接动态查询过滤。
    private readonly IWMSContextProvider _wmsContextProvider;

    /// <summary>
    /// 构造：带 WMS 上下文提供器的构造函数（运行时使用）。
    /// </summary>
    public WMSDbContext(
        DbContextOptions<WMSDbContext> options,
        IWMSContextProvider wmsContextProvider)
        : base(options)
    {
        _wmsContextProvider = wmsContextProvider;
    }

    /// <summary>
    /// 无上下文提供器的构造函数（用于某些迁移或测试场景）。
    /// </summary>
    public WMSDbContext(DbContextOptions<WMSDbContext> options)
        : base(options)
    {
    }

    /* 在此添加聚合根/实体对应的 DbSet 属性（仅 DbSet 声明，实体配置请放到 IEntityTypeConfiguration 中） */


    #region 从模块引入的实体（模块表）

    // Identity 模块
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }


    // Tenant Management 模块
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }


    // 用户与仓库关联表
    public DbSet<UserWarehouse> UserWarehouses { get; set; }

    #endregion
    
    #region 主数据模块 (MasterData)
    public DbSet<Warehouse> Warehouses { get; set; }
    public DbSet<Zone> Zones { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<CostCenter> CostCenters { get; set; }
    public DbSet<AccountAlias> AccountAliases { get; set; }
    #endregion

    #region 库存管理模块 (InventoryManage)
    public DbSet<TransferOrder> TransferOrders { get; set; }
    public DbSet<CycleCountOrder> CycleCountOrders { get; set; }
    public DbSet<CycleCountOrderDetail> CycleCountOrderDetails { get; set; }
    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<Container> Containers { get; set; }
    public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
    #endregion

    #region 任务模块 (TaskRouting)
    public DbSet<MoveTask> MoveTasks { get; set; }
    public DbSet<RoutingStrategy> RoutingStrategy { get; set; }
    #endregion
    
    #region 入库模块 (Inbound)
    public DbSet<ProductionInbound> ProductionInbounds { get; set; }
    public DbSet<ProductionInboundDetail> ProductionInboundDetails { get; set; }
    public DbSet<PurchaseOrder>  PurchaseOrders { get; set; }
    public DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
    public DbSet<PurchaseReceipt> PurchaseReceipts { get; set; }
    public DbSet<PurchaseReceiptDetail> PurchaseReceiptDetails { get; set; }
    public DbSet<PurchaseRecord> PurchaseRecords { get; set; }
    public DbSet<AdvancedShippingNotice> Asns { get; set; }
    public DbSet<AsnDetail> AsnDetails { get; set; }
    public DbSet<MiscInboundOrder> MiscInboundOrders { get; set; }
    public DbSet<MiscInboundOrderDetail> MiscInboundOrderDetails { get; set; }
    // ... 其他入库表
    #endregion
    
    #region 出库模块 (Outbound)
    public DbSet<MiscOutboundOrder> MiscOutboundOrders { get; set; }
    public DbSet<MiscOutboundOrderDetail> MiscOutboundOrderDetails { get; set; }
    public DbSet<SalesOrder> SalesOrders { get; set; }
    public DbSet<SalesOrderDetail> SalesOrderDetails { get; set; }
    public DbSet<SalesShipment> SalesShipments { get; set; }
    public DbSet<SalesShipmentDetail> SalesShipmentDetails { get; set; }
    public DbSet<SalesShipmentAllocation> SalesShipmentAllocations { get; set; }
    public DbSet<SalesShipmentRecord> SalesShipmentRecords { get; set; }
    public DbSet<WaveOrder> WaveOrders { get; set; }
    public DbSet<WaveOrderLine> WaveOrderLines { get; set; }
    public DbSet<PickList> PickLists { get; set; }
    public DbSet<PickListLine> PickListLines { get; set; }
    public DbSet<OutboundReviewOrder> OutboundReviewOrders { get; set; }
    public DbSet<OutboundReviewLine> OutboundReviewLines { get; set; }
    public DbSet<OutboundHandoverOrder> OutboundHandoverOrders { get; set; }
    public DbSet<OutboundHandoverLine> OutboundHandoverLines { get; set; }
    #endregion

    /// <summary>
    /// 全局模型创建：集中调用各模块的 Configure 方法并应用 IEntityTypeConfiguration 实现。
    /// </summary>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* 将需要的 ABP 模块表结构挂载到当前上下文（用于迁移） */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureFeatureManagement();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureTenantManagement();
        builder.ConfigureBlobStoring();

        // 挂载各自模块的实体映射配置
        builder.ConfigureMasterData();
        builder.ConfigureTaskRouting();
        builder.ConfigureInventory();
        builder.ConfigureInbound();
        builder.ConfigureOutBound();

        // 自动从当前程序集应用所有 IEntityTypeConfiguration<T>
        builder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);

        // 注意：不要在这里手动为每个实体写 HasQueryFilter，统一交由下面的过滤管道处理
    }

    // ========================================================================
    // 核心扩展：为 ABP 的过滤管道提供自定义隔离逻辑（部门 / 仓库）
    // 说明：重写以下方法可在 EF 层自动注入部门与仓库维度的 WHERE 条件
    // ========================================================================

    /// <summary>
    /// 指示是否为某实体启用自定义过滤（当实体实现 IMultiDepartment 或 IMultiWarehouse 时返回 true）。
    /// </summary>
    protected override bool ShouldFilterEntity<TEntity>(IMutableEntityType entityType) where TEntity : class
    {
        if (typeof(IMultiDepartment).IsAssignableFrom(typeof(TEntity))) return true;
        if (typeof(IMultiWarehouse).IsAssignableFrom(typeof(TEntity))) return true;

        return base.ShouldFilterEntity<TEntity>(entityType);
    }

    /// <summary>
    /// 为指定实体拼接额外的过滤表达式（与 ABP 的基础过滤表达式合并）。
    /// - 将部门过滤与仓库过滤以 AND 方式合并到已有过滤器上。
    /// </summary>
    protected override Expression<Func<TEntity, bool>> CreateFilterExpression<TEntity>(
        ModelBuilder modelBuilder,
        EntityTypeBuilder<TEntity> entityTypeBuilder) where TEntity : class
    {
        // 获取 ABP 已经构建的基础过滤表达式（例如：软删除等）
        var expression = base.CreateFilterExpression<TEntity>(modelBuilder, entityTypeBuilder);

        // 部门隔离：若实体实现 IMultiDepartment，则追加部门过滤条件
        if (typeof(IMultiDepartment).IsAssignableFrom(typeof(TEntity)))
        {
            Expression<Func<TEntity, bool>> deptFilter = e =>
                !_wmsContextProvider.CurrentDepartmentId.HasValue ||
                ((IMultiDepartment)e).DepartmentId == _wmsContextProvider.CurrentDepartmentId;

            expression = expression == null ? deptFilter : CombineExpressions(expression, deptFilter);
        }

        // 仓库隔离：若实体实现 IMultiWarehouse，则追加仓库过滤条件
        if (typeof(IMultiWarehouse).IsAssignableFrom(typeof(TEntity)))
        {
            Expression<Func<TEntity, bool>> whFilter = e =>
                !_wmsContextProvider.CurrentWarehouseId.HasValue ||
                ((IMultiWarehouse)e).WarehouseId == _wmsContextProvider.CurrentWarehouseId;

            expression = expression == null ? whFilter : CombineExpressions(expression, whFilter);
        }

        return expression;
    }

    /// <summary>
    /// 将两个表达式通过逻辑 AND 合并为单个表达式（参数替换以确保参数一致）。
    /// </summary>
    protected virtual Expression<Func<T, bool>> CombineExpressions<T>(
        Expression<Func<T, bool>> expression1,
        Expression<Func<T, bool>> expression2)
    {
        var parameter = Expression.Parameter(typeof(T), "e");

        var leftVisitor = new ParameterReplaceVisitor(expression1.Parameters[0], parameter);
        var left = leftVisitor.Visit(expression1.Body);

        var rightVisitor = new ParameterReplaceVisitor(expression2.Parameters[0], parameter);
        var right = rightVisitor.Visit(expression2.Body);

        return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left, right), parameter);
    }
}

/// <summary>
/// 辅助工具：表达式参数替换器，用于将两个 Lambda 的参数统一为同一个 ParameterExpression，
/// 以便将它们合并为单个表达式。此类实现为 internal，避免被外部直接使用。
/// </summary>
internal class ParameterReplaceVisitor : ExpressionVisitor
{
    private readonly ParameterExpression _oldParameter;
    private readonly ParameterExpression _newParameter;

    public ParameterReplaceVisitor(ParameterExpression oldParameter, ParameterExpression newParameter)
    {
        _oldParameter = oldParameter;
        _newParameter = newParameter;
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
        // 如果当前遍历到的参数等于旧参数，则返回新的统一参数，确保表达式参数一致性
        if (ReferenceEquals(node, _oldParameter))
        {
            return _newParameter;
        }

        return base.VisitParameter(node);
    }
}