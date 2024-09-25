using Mahak.Main.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Mahak.Main.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class MainController : AbpControllerBase
{
    protected MainController()
    {
        LocalizationResource = typeof(MainResource);
    }
}
