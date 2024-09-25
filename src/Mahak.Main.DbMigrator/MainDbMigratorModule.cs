using Mahak.Main.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Mahak.Main.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(MainEntityFrameworkCoreModule),
    typeof(MainApplicationContractsModule)
)]
public class MainDbMigratorModule : AbpModule
{
}
