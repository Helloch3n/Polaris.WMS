using Polaris.WMS.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Polaris.WMS.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class WMSController : AbpControllerBase
{
    protected WMSController()
    {
        LocalizationResource = typeof(WMSResource);
    }
}

