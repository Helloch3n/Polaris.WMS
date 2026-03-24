using Volo.Abp.Settings;

namespace Polaris.WMS.Settings;

public class WMSSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(WMSSettings.MySetting1));
    }
}

