using Microsoft.EntityFrameworkCore;

namespace Muwesome.Persistence.EntityFramework {
  public class PersistenceContextProvider : IPersistenceContextProvider {
    private readonly DbContextOptions contextOptions;

    /// <summary>Initializes a new instance of the <see cref="PersistenceContextProvider"/> class.</summary>
    public PersistenceContextProvider(ConnectionConfiguration connectionConfig) {
      this.contextOptions = connectionConfig.ToDbContextOptions();
    }

    /// <summary>Creates a new persistence context.</summary>
    public IContext CreateContext() =>
      new PersistenceContext(new DatabaseContext(this.contextOptions));

    /// <summary>Applies any pending updates to the underlying storage engine.</summary>
    public void ApplyPendingStorageUpdates() {
      using (var updateContext = new DatabaseContext(contextOptions)) {
        updateContext.Database.Migrate();
      }
    }

    /// <summary>Applies any pending updates to the underlying storage engine.</summary>
    public void RecreateStorage() {
      using (var createContext = new DatabaseContext(contextOptions)) {
        createContext.Database.EnsureDeleted();
        createContext.Database.EnsureCreated();
      }
    }
  }
}
