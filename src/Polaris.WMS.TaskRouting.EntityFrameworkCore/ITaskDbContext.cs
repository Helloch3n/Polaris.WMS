using Microsoft.EntityFrameworkCore;
using Polaris.WMS.TaskRouting.Domain.LogisticsStrategies;
using Polaris.WMS.TaskRouting.Domain.MoveTasks;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Polaris.WMS.TaskRouting.EntityFrameworkCore;

[ConnectionStringName("Default")]
public interface ITaskDbContext : IEfCoreDbContext
{
}