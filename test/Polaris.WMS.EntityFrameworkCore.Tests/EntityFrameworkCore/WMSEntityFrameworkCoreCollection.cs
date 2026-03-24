using Xunit;

namespace Polaris.WMS.EntityFrameworkCore;

[CollectionDefinition(WMSTestConsts.CollectionDefinitionName)]
public class WMSEntityFrameworkCoreCollection : ICollectionFixture<WMSEntityFrameworkCoreFixture>
{

}

