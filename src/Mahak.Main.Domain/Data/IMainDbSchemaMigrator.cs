using System.Threading.Tasks;

namespace Mahak.Main.Data;

public interface IMainDbSchemaMigrator
{
    Task MigrateAsync();
}
