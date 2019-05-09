using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Muwesome.Persistence.EntityFramework {
  internal class PersistenceContext : IContext {
    private readonly DbContext context;

    /// <summary>Initializes a new instance of the <see cref="PersistenceContext"/> class.</summary>
    public PersistenceContext(DbContext context) => this.context = context;

    /// <inheritdoc />
    public void Attach(object item) => this.context.Attach(item);

    /// <inheritdoc />
    public void Detach(object item) {
      var entry = this.context.Entry(item);
      if (entry != null) {
        entry.State = EntityState.Detached;
      }
    }

    /// <inheritdoc />
    public TEntity Create<TEntity>(params object[] args)
        where TEntity : class {
      var entity = args.Length > 0
        ? Activator.CreateInstance(typeof(TEntity), args)
        : Activator.CreateInstance(typeof(TEntity));

      if (entity != null) {
        this.context.Add(entity);
      }

      return entity as TEntity;
    }

    /// <inheritdoc />
    public TEntity GetById<TEntity>(Guid id)
        where TEntity : class => this.context.Set<TEntity>().Find(id);

    /// <inheritdoc />
    public bool Delete<TEntity>(TEntity entity)
        where TEntity : class => this.context.Remove(entity) != null;

    /// <inheritdoc />
    public void SaveChanges() => this.context.SaveChanges();

    /// <inheritdoc />
    public Task SaveChangesAsync() => this.context.SaveChangesAsync();

    /// <inheritdoc />
    public void Dispose() => this.context?.Dispose();
  }
}