using System;
using Microsoft.EntityFrameworkCore;

namespace Muwesome.Persistence.EntityFramework {
  /// <summary>The available storage engines.</summary>
  public enum StorageEngine {
    /// <summary>The SQLite file database.</summary>
    Sqlite,
  }

  /// <summary>A connection configuration.</summary>
  public class ConnectionConfiguration {
    /// <summary>Gets or sets the context type.</summary>
    public string ContextType { get; set; }

    /// <summary>Gets or sets the connection string.</summary>
    public string ConnectionString { get; set; }

    /// <summary>Gets or sets the storage engine.</summary>
    public StorageEngine StorageEngine { get; set; }

    /// <summary>Returns a <see cref="DbContextOptions" /> representation of the connection config.</summary>
    internal DbContextOptions ToDbContextOptions() {
      var contextBuilder = new DbContextOptionsBuilder();

      switch (this.StorageEngine) {
        case StorageEngine.Sqlite:
          contextBuilder.UseSqlite(this.ConnectionString);
          break;
        default:
          throw new ArgumentException(nameof(this.StorageEngine));
      }

      return contextBuilder.Options;
    }
  }
}