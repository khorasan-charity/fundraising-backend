using Volo.Abp.Modularity;

namespace Mahak.Main;

[DependsOn(
    typeof(MainApplicationModule),
    typeof(MainDomainTestModule)
)]
public class MainApplicationTestModule : AbpModule
{

}
