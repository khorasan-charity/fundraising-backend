using Volo.Abp.Modularity;

namespace Mahak.Main;

[DependsOn(
    typeof(MainDomainModule),
    typeof(MainTestBaseModule)
)]
public class MainDomainTestModule : AbpModule
{

}
