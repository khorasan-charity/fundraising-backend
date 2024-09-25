using Mahak.Main.Localization;
using Volo.Abp.Application.Services;
using Volo.Abp.ObjectMapping;

namespace Mahak.Main;

/* Inherit your application services from this class.
 */
public abstract class MainAppService : ApplicationService
{
    protected MainAppService()
    {
        LocalizationResource = typeof(MainResource);
    }

    protected T? MapTo<T>(object? source) where T : class
    {
        if (source is null)
        {
            return null;
        }

        return (T)ObjectMapper.Map(source.GetType(), typeof(T), source);
    }
}