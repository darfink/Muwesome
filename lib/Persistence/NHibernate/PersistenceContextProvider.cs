using System.ComponentModel;

namespace Muwesome.Persistence.NHibernate {
  public class PersistenceContextProvider : IPersistenceContextProvider {
    private readonly SessionProvider sessionProvider;

    /// <summary>Initializes a new instance of the <see cref="PersistenceContextProvider"/> class.</summary>
    public PersistenceContextProvider(PersistenceConfiguration config) {
      this.sessionProvider = new SessionProvider(config.StorageEngine, config.ConnectionString);
      this.ExecuteSchemaOperation(config.SchemaOperation);
    }

    /// <inheritdoc />
    public IContext CreateContext() =>
      new PersistenceContext(this.sessionProvider.CreateSession());

    /// <inheritdoc />
    public IAccountContext CreateAccountContext() =>
      new AccountContext(this.sessionProvider.CreateSession());

    /// <inheritdoc />
    public void Dispose() => this.sessionProvider.Dispose();

    private void ExecuteSchemaOperation(SchemaOperation schemaOperation) {
      switch (schemaOperation) {
        case SchemaOperation.None:
          break;
        case SchemaOperation.Update:
          this.sessionProvider.UpdateSchema();
          break;
        case SchemaOperation.Recreate:
          this.sessionProvider.RecreateSchema();
          break;
        default:
          throw new InvalidEnumArgumentException(nameof(schemaOperation), (int)schemaOperation, typeof(SchemaOperation));
      }
    }
  }
}