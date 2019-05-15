using System.ComponentModel;
using System.Data.Common;
using System.Reflection;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using Muwesome.Persistence.NHibernate.Conventions;
using Muwesome.Persistence.NHibernate.Utility;
using NHibernate;
using NHibernate.Tool.hbm2ddl;

namespace Muwesome.Persistence.NHibernate {
  using DbConfiguration = global::NHibernate.Cfg.Configuration;

  public class PersistenceContextProvider : IPersistenceContextProvider {
    private readonly PersistenceConfiguration config;
    private readonly ISessionFactory sessionFactory;

    /// <summary>Initializes a new instance of the <see cref="PersistenceContextProvider"/> class.</summary>
    public PersistenceContextProvider(PersistenceConfiguration config) {
      this.config = config;
      var databaseConfig = this.CreateDatabaseConfiguration();
      this.sessionFactory = databaseConfig.BuildSessionFactory();
      this.ExecuteSchemaOperations(databaseConfig);
    }

    /// <inheritdoc />
    public IContext CreateContext() => new PersistenceContext(this.CreateSession());

    /// <inheritdoc />
    public IAccountContext CreateAccountContext() => new AccountContext(this.CreateSession());

    /// <inheritdoc />
    public void Dispose() => this.sessionFactory.Dispose();

    /// <summary>Creates a manually controlled session.</summary>
    private ISession CreateSession() {
      var session = this.sessionFactory.OpenSession();
      session.FlushMode = FlushMode.Manual;
      session.Disconnect();
      return session;
    }

    /// <summary>Creates a database configuration.</summary>
    private DbConfiguration CreateDatabaseConfiguration() =>
      Fluently
        .Configure()
        .Database(this.CreatePersistenceConfigurer())
        .Mappings(m => m.AutoMappings.Add(
          AutoMap
            .AssemblyOf<Muwesome.DomainModel.Identifiable>(new DatabaseMappingConfiguration())
            .Conventions.Setup(c => {
              c.Add(ForeignKey.EndsWith("Id"));
              c.AddAssembly(Assembly.GetExecutingAssembly());
            })
            .UseOverridesFromAssembly(Assembly.GetExecutingAssembly())))
            .BuildConfiguration();

    private IPersistenceConfigurer CreatePersistenceConfigurer() {
      switch (this.config.StorageEngine) {
        case StorageEngine.InMemory:
          return SQLiteConfiguration.Standard.InMemory().Provider<InMemoryConnectionProvider>();
        case StorageEngine.Sqlite:
          return SQLiteConfiguration.Standard.ConnectionString(this.config.ConnectionString);
        default:
          throw new InvalidEnumArgumentException(nameof(this.config.StorageEngine), (int)this.config.StorageEngine, typeof(StorageEngine));
      }
    }

    private void ExecuteSchemaOperations(DbConfiguration dbConfig) {
      switch (this.config.SchemaOperation) {
        case SchemaOperation.None:
          break;
        case SchemaOperation.Update:
          // This seems to be the only way to reuse a connection provider (required for the in-memory database)
          var sessionFactoryImpl = (global::NHibernate.Impl.SessionFactoryImpl)this.sessionFactory;
          new SchemaUpdate(dbConfig, sessionFactoryImpl.Settings).Execute(useStdOut: false, doUpdate: true);
          break;
        case SchemaOperation.Recreate:
          using (var session = this.sessionFactory.OpenSession()) {
            new SchemaExport(dbConfig).Execute(useStdOut: false, execute: true, justDrop: false, session.Connection, null);
          }

          break;
        default:
          throw new InvalidEnumArgumentException(nameof(this.config.SchemaOperation), (int)this.config.SchemaOperation, typeof(SchemaOperation));
      }
    }
  }
}