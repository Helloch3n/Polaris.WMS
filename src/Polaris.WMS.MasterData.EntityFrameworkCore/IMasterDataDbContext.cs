using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Polaris.WMS.MasterData.EntityFrameworkCore;

[ConnectionStringName("Default")]
public interface IMasterDataDbContext : IEfCoreDbContext
{
}