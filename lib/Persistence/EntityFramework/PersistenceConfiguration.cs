using System;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Muwesome.Persistence.EntityFramework {
  /// <summary>The available storage engines.</summary>
  public enum StorageEngine {
    /// <summary>An in-memory database.</summary>
    /// <remarks>The default implementation uses an SQLite in-memory database.</remarks>
    InMemory,

    /// <summary>An SQLite file database.</summary>
    Sqlite,
  }

  /// <summary>A persistence configuration.</summary>
  public class PersistenceConfiguration {
    /// <summary>Gets or sets the connection string.</summary>
    public string ConnectionString { get; set; }

    /// <summary>Gets or sets the storage engine.</summary>
    public StorageEngine StorageEngine { get; set; }

    /// <summary>Creates an in-memory configuration.</summary>
    public static PersistenceConfiguration InMemory() =>
      new PersistenceConfiguration { StorageEngine = StorageEngine.InMemory };

    /// <summary>Returns a <see cref="DbContextOptions" /> representation of the connection config.</summary>
    internal DbContextOptions ToDbContextOptions(out DbConnection connection) {
      var contextBuilder = new DbContextOptionsBuilder();

      switch (this.StorageEngine) {
        case StorageEngine.Sqlite:
          contextBuilder.UseSqlite(this.ConnectionString);
          connection = null;
          break;
        case StorageEngine.InMemory:
          connection = new SqliteConnection("DataSource=:memory:");
          connection.Open();
          contextBuilder.UseSqlite(connection);
          break;
        default:
          throw new ArgumentException(nameof(this.StorageEngine));
      }

      return contextBuilder.Options;
    }
  }
}