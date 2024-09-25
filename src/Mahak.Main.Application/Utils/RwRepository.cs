using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Mahak.Main.Utils;

public class RwRepository<TEntity, TId>(IAbpLazyServiceProvider serviceProvider)
    : IRwRepository<TEntity, TId> where TEntity : class, IEntity<TId>
{
    public IRepository<TEntity, TId> ReadWrite =>
        serviceProvider.LazyGetRequiredService<IRepository<TEntity, TId>>();

    public IReadOnlyRepository<TEntity, TId> ReadOnly =>
        serviceProvider.LazyGetRequiredService<IReadOnlyRepository<TEntity, TId>>();
}

public interface IRwRepository<TEntity, TId> : ITransientDependency where TEntity : class, IEntity<TId>
{
    IRepository<TEntity, TId> ReadWrite { get; }

    IReadOnlyRepository<TEntity, TId> ReadOnly { get; }
}