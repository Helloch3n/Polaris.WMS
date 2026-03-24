using Polaris.WMS.Samples;
using Xunit;

namespace Polaris.WMS.EntityFrameworkCore.Domains;

[Collection(WMSTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<WMSEntityFrameworkCoreTestModule>
{

}

