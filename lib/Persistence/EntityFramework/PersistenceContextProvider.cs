using Microsoft.EntityFrameworkCore;

namespace Muwesome.Persistence.EntityFramework {
  public class PersistenceContextProvider : IPersistenceContextProvider {
    private readonly DbContextOptions contextOptions;

    /// <summary>Initializes a new instance of the <see cref="PersistenceContextProvider"/> class.</summary>
    public PersistenceContextProvider(ConnectionConfiguration connectionConfig) {
      this.contextOptions = connectionConfig.ToDbContextOptions();
    }

    /// <inheritdoc />
    public IContext CreateContext() =>
      new PersistenceContext(new DatabaseContext(this.contextOptions));

    /// <inheritdoc />
    public IAccountContext CreateAccountContext() =>
      new AccountContext(new DatabaseContext(this.contextOptions));

    /// <summary>Applies any pending updates to the underlying storage engine.</summary>
    public void ApplyPendingStorageUpdates() {
      using (var updateContext = new DatabaseContext(this.contextOptions)) {
        updateContext.Database.Migrate();
      }
    }

    /// <summary>Applies any pending updates to the underlying storage engine.</summary>
    public void RecreateStorage() {
      using (var createContext = new DatabaseContext(this.contextOptions)) {
        createContext.Database.EnsureDeleted();
        createContext.Database.EnsureCreated();
      }
    }
  }
}