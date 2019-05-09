using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Muwesome.Persistence.EntityFramework {
  internal class PersistenceContext : IContext {
    /// <summary>Initializes a new instance of the <see cref="PersistenceContext"/> class.</summary>
    public PersistenceContext(DbContext context) => this.Context = context;

    /// <summary>Gets the database context.</summary>
    protected DbContext Context { get; private set; }

    /// <inheritdoc />
    public void Attach(object item) => this.Context.Attach(item);

    /// <inheritdoc />
    public void Detach(object item) {
      var entry = this.Context.Entry(item);
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
        this.Context.Add(entity);
      }

      return entity as TEntity;
    }

    /// <inheritdoc />
    public TEntity GetById<TEntity>(Guid id)
        where TEntity : class => this.Context.Set<TEntity>().Find(id);

    /// <inheritdoc />
    public IEnumerable<TEntity> GetAll<TEntity>()
      where TEntity : class {
      return this.Context.Set<TEntity>();
    }

    /// <inheritdoc />
    public bool Delete<TEntity>(TEntity entity)
        where TEntity : class => this.Context.Remove(entity) != null;

    /// <inheritdoc />
    public void SaveChanges() => this.Context.SaveChanges();

    /// <inheritdoc />
    public Task SaveChangesAsync() => this.Context.SaveChangesAsync();

    /// <inheritdoc />
    public void Dispose() => this.Context?.Dispose();
  }
}