using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Muwesome.Persistence.NHibernate.Utility;
using NHibernate;
using NHibernate.Linq;

namespace Muwesome.Persistence.NHibernate {
  internal class PersistenceContext : IContext {
    /// <summary>Initializes a new instance of the <see cref="PersistenceContext"/> class.</summary>
    public PersistenceContext(ISession session) => this.Session = session;

    /// <summary>Gets the database context.</summary>
    protected ISession Session { get; private set; }

    /// <inheritdoc />
    public void Attach(object item) => this.Session.Lock(item, LockMode.None);

    /// <inheritdoc />
    public void Detach(object item) => this.Session.Evict(item);

    /// <inheritdoc />
    public TEntity Create<TEntity>(params object[] args)
        where TEntity : class {
      var entity = args.Length > 0
        ? Activator.CreateInstance(typeof(TEntity), args)
        : Activator.CreateInstance(typeof(TEntity));

      if (entity != null) {
        this.Session.Persist(entity);
      }

      return entity as TEntity;
    }

    /// <inheritdoc />
    public TEntity GetById<TEntity>(Guid id)
        where TEntity : class {
      using (this.WithConnection()) {
        return this.Session.Get<TEntity>(id);
      }
    }

    /// <inheritdoc />
    public async Task<TEntity> GetByIdAsync<TEntity>(Guid id)
        where TEntity : class {
      using (this.WithConnection()) {
        return await this.Session.GetAsync<TEntity>(id);
      }
    }

    /// <inheritdoc />
    public IEnumerable<TEntity> GetAll<TEntity>()
        where TEntity : class {
      using (this.WithConnection()) {
        return this.Session.QueryOver<TEntity>().List();
      }
    }

    /// <inheritdoc />
    public void Delete<TEntity>(TEntity entity)
        where TEntity : class {
      this.Session.Delete(entity);
    }

    /// <inheritdoc />
    public void SaveChanges() {
      using (this.WithConnection()) {
        this.Session.Flush();
      }
    }

    /// <inheritdoc />
    public async Task SaveChangesAsync() {
      using (this.WithConnection()) {
        await this.Session.FlushAsync();
      }
    }

    /// <inheritdoc />
    public void Dispose() => this.Session.Dispose();

    /// <summary>Establishes a connection with a guard.</summary>
    protected IDisposable WithConnection() {
      this.Session.Reconnect();
      ITransaction transaction = null;
      try {
        transaction = this.Session.BeginTransaction();
      } catch (Exception) {
        this.Session.Disconnect();
        throw;
      }

      return new ScopeGuard<(ISession Session, ITransaction Transaction)>(
        (this.Session, transaction),
        (context) => {
          try {
            context.Transaction.Commit();
          } finally {
            context.Transaction.Dispose();
            context.Session.Disconnect();
          }
        });
    }
  }
}