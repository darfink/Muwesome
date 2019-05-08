using System;
using System.Threading.Tasks;

namespace Muwesome.Persistence {
  /// <summary>A persistence context.</summary>
  public interface IContext : IDisposable {
    /// <summary>Attaches an item to the context.</summary>
    void Attach(object item);

    /// <summary>Detaches an item to the context.</summary>
    void Detach(object item);

    /// <summary>Creates a new entity of the specified type.</summary>
    TEntity Create<TEntity>(params object[] args) where TEntity : class;

    /// <summary>Retrieves an entity by ID.</summary>
    TEntity GetById<TEntity>(Guid id) where TEntity : class;

    /// <summary>Deletes an entity.</summary>
    bool Delete<TEntity>(TEntity entity) where TEntity : class;

    /// <summary>Saves any pending changes.</summary>
    void SaveChanges();

    /// <summary>Saves any pending changes asynchronously.</summary>
    Task SaveChangesAsync();
  }
}