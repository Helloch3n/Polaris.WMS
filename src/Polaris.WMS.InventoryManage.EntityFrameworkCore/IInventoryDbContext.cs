using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Polaris.WMS.InventoryManage.EntityFrameworkCore;

[ConnectionStringName("Default")]
public interface IInventoryDbContext : IEfCoreDbContext
{
}