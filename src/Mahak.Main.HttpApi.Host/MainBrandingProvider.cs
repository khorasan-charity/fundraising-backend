using Microsoft.Extensions.Localization;
using Mahak.Main.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace Mahak.Main;

[Dependency(ReplaceServices = true)]
public class MainBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<MainResource> _localizer;

    public MainBrandingProvider(IStringLocalizer<MainResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
