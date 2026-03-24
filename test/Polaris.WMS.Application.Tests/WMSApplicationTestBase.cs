using Volo.Abp.Modularity;

namespace Polaris.WMS;

public abstract class WMSApplicationTestBase<TStartupModule> : WMSTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}

