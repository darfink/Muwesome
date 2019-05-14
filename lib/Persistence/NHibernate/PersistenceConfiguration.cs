namespace Muwesome.Persistence.NHibernate {
  /// <summary>Represents all available schema operations.</summary>
  public enum SchemaOperation {
    /// <summary>No operations is performed.</summary>
    None,

    /// <summary>Outdated schemas are updated.</summary>
    Update,

    /// <summary>All schemas are recreated.</summary>
    Recreate,
  }

  /// <summary>Represents all available storage engines.</summary>
  public enum StorageEngine {
    /// <summary>An in-memory database.</summary>
    /// <remarks>The default implementation uses an SQLite in-memory database.</remarks>
    InMemory,

    /// <summary>An SQLite file database.</summary>
    Sqlite,
  }

  /// <summary>A persistence configuration.</summary>
  public class PersistenceConfiguration {
    public PersistenceConfiguration(StorageEngine engine) =>
      this.StorageEngine = engine;

    /// <summary>Gets or sets the connection string.</summary>
    public string ConnectionString { get; set; }

    /// <summary>Gets or sets the storage engine.</summary>
    public StorageEngine StorageEngine { get; set; }

    /// <summary>Gets or sets the schema operation.</summary>
    public SchemaOperation SchemaOperation { get; set; }

    /// <summary>Creates an in-memory configuration.</summary>
    public static PersistenceConfiguration InMemory() =>
      new PersistenceConfiguration(StorageEngine.InMemory) {
        SchemaOperation = SchemaOperation.Recreate,
      };
  }
}