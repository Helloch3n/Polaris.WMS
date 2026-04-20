using Microsoft.EntityFrameworkCore;
using Polaris.WMS.MasterData.Domain.AccountAliases;
using Polaris.WMS.MasterData.Domain.CostCenters;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Polaris.WMS.MasterData.EntityFrameworkCore;

[ConnectionStringName("Default")]
public interface IMasterDataDbContext : IEfCoreDbContext
{
}