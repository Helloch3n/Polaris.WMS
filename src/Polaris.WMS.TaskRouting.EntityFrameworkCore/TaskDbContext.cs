using Microsoft.EntityFrameworkCore;
using Polaris.WMS.TaskRouting.Domain.LogisticsStrategies;
using Polaris.WMS.TaskRouting.Domain.MoveTasks;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Polaris.WMS.TaskRouting.EntityFrameworkCore;

// 指定连接字符串的名字。默认叫 "Default"，
// 如果未来主数据要用独立数据库，只需要在 appsettings.json 里加一个 "MasterData" 的连接串即可。
[ConnectionStringName("Default")]
public class TaskDbContext : AbpDbContext<TaskDbContext>, ITaskDbContext
{
    // 声明当前模块管辖的实体
    //Tasks
    public DbSet<MoveTask> MoveTasks { get; set; }

    //routing
    public DbSet<RoutingStrategy> RoutingStrategy { get; set; }

    public TaskDbContext(DbContextOptions<TaskDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureTaskRouting();
    }
}