using System.Threading.Tasks;

namespace Polaris.WMS.Data;

public interface IWMSDbSchemaMigrator
{
    Task MigrateAsync();
}

