using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Mahak.Main.Data;

/* This is used if database provider does't define
 * IMainDbSchemaMigrator implementation.
 */
public class NullMainDbSchemaMigrator : IMainDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
