using Volo.Abp.Settings;

namespace Mahak.Main.Settings;

public class MainSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        context.Add(new SettingDefinition(MainSettings.FileStoragePath, "../files"));
    }
}