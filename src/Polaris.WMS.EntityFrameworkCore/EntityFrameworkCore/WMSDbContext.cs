using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders; // ABP 10 必须引入的构建器命名空间
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polaris.WMS.DataSync;
using Polaris.WMS.Inbound;
using Polaris.WMS.Inventorys;
using Polaris.WMS.Isolation;
using Polaris.WMS.MasterData;
using Polaris.WMS.MasterData.Reels;
using Polaris.WMS.Outbound;
using Polaris.WMS.ProductionInbounds;
using Polaris.WMS.Putaway;
using Polaris.WMS.TransferOrders;
using Polaris.WMS.Users;
using System;
using System.Linq;
using System.Linq.Expressions;
using Polaris.WMS.Inbound.EntityFrameworkCore;
using Polaris.WMS.InventoryManage.Domain.inventories;
using Polaris.WMS.InventoryManage.Domain.Reels;
using Polaris.WMS.InventoryManage.Domain.TransferOrders;
using Polaris.WMS.InventoryManage.EntityFrameworkCore;
using Polaris.WMS.MasterData.Domain.Locations;
using Polaris.WMS.MasterData.Domain.warehouses;
using Polaris.WMS.MasterData.Domain.Zones;
using Polaris.WMS.MasterData.EntityFrameworkCore;
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

// [ReplaceDbContext(typeof(IIdentityDbContext))]
// [ReplaceDbContext(typeof(ITenantManagementDbContext))]
// [ReplaceDbContext(typeof(MasterDataDbContext))]
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
    private readonly IWMSContextProvider _wmsContextProvider;

    public WMSDbContext(
        DbContextOptions<WMSDbContext> options,
        IWMSContextProvider wmsContextProvider)
        : base(options)
    {
        _wmsContextProvider = wmsContextProvider;
    }

    public WMSDbContext(DbContextOptions<WMSDbContext> options)
        : base(options)
    {
    }

    /* Add DbSet properties for your Aggregate Roots / Entities here. */


    #region Entities from the modules

    // Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }


    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }


    // Users - Warehouse mapping
    public DbSet<UserWarehouse> UserWarehouses { get; set; }

    #endregion
    
    #region 主数据模块 (MasterData)
    public DbSet<Warehouse> Warehouses { get; set; }
    public DbSet<Zone> Zones { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Product> Products { get; set; }
    #endregion

    #region 库存管理模块 (InventoryManage)
    public DbSet<TransferOrder> TransferOrders { get; set; }
    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<Reel> Reels { get; set; }
    #endregion

    #region 任务模块 (TaskRouting)
    public DbSet<MoveTask> MoveTasks { get; set; }
    public DbSet<RoutingStrategy> RoutingStrategy { get; set; }
    #endregion

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureFeatureManagement();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureTenantManagement();
        builder.ConfigureBlobStoring();

        // 挂载主数据模块的表结构
        builder.ConfigureMasterData();
        builder.ConfigureTaskRouting();
        builder.ConfigureInventory();
        builder.ConfigureInbound();
        builder.ConfigureOutBound();

        // 自动扫描当前程序集 (Assembly) 中所有实现了 IEntityTypeConfiguration<T> 的类并将它们应用到 builder 中。
        builder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);

        // 注意：此处不再手写 foreach HasQueryFilter，将控制权交给下方的原生过滤管道
    }

    // ========================================================================
    // 🌟 核心改造区：重写 ABP 10 原生过滤管道 (带 TEntity : class 泛型约束)
    // ========================================================================

    /// <summary>
    /// 1. 告诉 ABP：遇到实现这两个接口的实体时，启用自定义过滤器
    /// </summary>
    protected override bool ShouldFilterEntity<TEntity>(IMutableEntityType entityType) where TEntity : class
    {
        if (typeof(IMultiDepartment).IsAssignableFrom(typeof(TEntity))) return true;
        if (typeof(IMultiWarehouse).IsAssignableFrom(typeof(TEntity))) return true;

        return base.ShouldFilterEntity<TEntity>(entityType);
    }

    /// <summary>
    /// 2. 缝合表达式：将 ABP 原生过滤（如软删除 IsDeleted==false）和我们的隔离墙拼装在一起
    /// </summary>
    protected override Expression<Func<TEntity, bool>> CreateFilterExpression<TEntity>(
        ModelBuilder modelBuilder,
        EntityTypeBuilder<TEntity> entityTypeBuilder) where TEntity : class
    {
        // 拿到 ABP 已经拼好的基础过滤墙（传入 ABP 10 所需的构建器参数）
        var expression = base.CreateFilterExpression<TEntity>(modelBuilder, entityTypeBuilder);

        // 挂载 部门/车间 隔离墙
        if (typeof(IMultiDepartment).IsAssignableFrom(typeof(TEntity)))
        {
            Expression<Func<TEntity, bool>> deptFilter = e =>
                !_wmsContextProvider.CurrentDepartmentId.HasValue ||
                ((IMultiDepartment)e).DepartmentId == _wmsContextProvider.CurrentDepartmentId;

            expression = expression == null ? deptFilter : CombineExpressions(expression, deptFilter);
        }

        // 挂载 仓库 隔离墙
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
    /// 3. 表达式树缝合工具（将多个 WHERE 条件用 AND 安全连接）
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
/// 4. C# 原生表达式参数替换器 (极其稳定，不依赖任何 EF Core 版本)
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
        // 如果遍历到的参数节点是我们想要替换的老参数，就返回新参数
        if (ReferenceEquals(node, _oldParameter))
        {
            return _newParameter;
        }

        return base.VisitParameter(node);
    }
}