using Volo.Abp.Modularity;

namespace Mahak.Main;

public abstract class MainApplicationTestBase<TStartupModule> : MainTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
