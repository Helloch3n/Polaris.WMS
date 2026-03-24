using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Polaris.WMS.Outbound.EntityFrameworkCore;

[ConnectionStringName("Default")]
public interface IOutBoundDbContext : IEfCoreDbContext
{
}