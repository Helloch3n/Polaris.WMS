using Polaris.WMS.Samples;
using Xunit;

namespace Polaris.WMS.EntityFrameworkCore.Applications;

[Collection(WMSTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<WMSEntityFrameworkCoreTestModule>
{

}

