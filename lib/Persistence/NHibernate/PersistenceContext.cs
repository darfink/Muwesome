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
      using (this.Connect()) {
        return this.Session.Get<TEntity>(id);
      }
    }

    /// <inheritdoc />
    public async Task<TEntity> GetByIdAsync<TEntity>(Guid id)
        where TEntity : class {
      using (this.Connect()) {
        return await this.Session.GetAsync<TEntity>(id);
      }
    }

    /// <inheritdoc />
    public IEnumerable<TEntity> GetAll<TEntity>()
        where TEntity : class {
      using (this.Connect()) {
        return this.Session.Query<TEntity>().ToListAsync().Result;
      }
    }

    /// <inheritdoc />
    public bool Delete<TEntity>(TEntity entity)
        where TEntity : class {
      // TODO: Determine result of operation
      this.Session.Delete(entity);
      return true;
    }

    /// <inheritdoc />
    public void SaveChanges() {
      using (this.Connect()) {
        this.Session.Flush();
      }
    }

    /// <inheritdoc />
    public async Task SaveChangesAsync() {
      using (this.Connect()) {
        await this.Session.FlushAsync();
      }
    }

    /// <inheritdoc />
    public void Dispose() => this.Session.Dispose();

    /// <summary>Establishes a connection with a guard.</summary>
    protected IDisposable Connect() {
      this.Session.Reconnect();
      return new ScopeGuard<ISession>(this.Session, session => session.Disconnect());
    }
  }
}