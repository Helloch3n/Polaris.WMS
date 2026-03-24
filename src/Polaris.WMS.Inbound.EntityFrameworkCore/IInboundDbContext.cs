using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Polaris.WMS.Inbound.EntityFrameworkCore;

[ConnectionStringName("Default")]
public interface IInboundDbContext : IEfCoreDbContext
{
}