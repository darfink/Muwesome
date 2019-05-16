using System;
using System.ComponentModel;
using System.Reflection;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Helpers;
using Muwesome.Persistence.NHibernate.Conventions;
using Muwesome.Persistence.NHibernate.Utility;
using NHibernate;
using NHibernate.Cache;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace Muwesome.Persistence.NHibernate {
  internal class SessionProvider : IDisposable {
    private readonly ISessionFactory sessionFactory;
    private readonly Configuration config;

    /// <summary>Initializes a new instance of the <see cref="SessionProvider"/> class.</summary>
    public SessionProvider(StorageEngine storageEngine, string connectionString = null) {
      this.config = this.CreateDatabaseConfiguration(this.CreatePersistenceConfigurer(storageEngine, connectionString));
      this.sessionFactory = this.config.BuildSessionFactory();
    }

    /// <summary>Creates a manually controlled session.</summary>
    public ISession CreateSession() {
      var session = this.sessionFactory.WithOptions().FlushMode(FlushMode.Manual).OpenSession();
      session.Disconnect();
      return session;
    }

    /// <summary>Recreates the schema.</summary>
    public void RecreateSchema() {
      using (var session = this.sessionFactory.OpenSession()) {
        new SchemaExport(this.config).Execute(useStdOut: false, execute: true, justDrop: false, session.Connection, null);
      }
    }

    /// <summary>Updates the schema.</summary>
    public void UpdateSchema() {
      // This seems to be the only way to reuse a connection provider (required for the in-memory database)
      var sessionFactoryImpl = (global::NHibernate.Impl.SessionFactoryImpl)this.sessionFactory;
      new SchemaUpdate(this.config, sessionFactoryImpl.Settings).Execute(useStdOut: false, doUpdate: true);
    }

    /// <inheritdoc />
    public void Dispose() {
      // TODO: Remove this after additional DB integration has been performed
      System.Console.WriteLine(this.sessionFactory.Statistics.GetSecondLevelCacheStatistics(CacheConfigurationConvention.RegionName));
      this.sessionFactory.Dispose();
    }

    /// <summary>Creates a database configuration.</summary>
    private Configuration CreateDatabaseConfiguration(IPersistenceConfigurer persistenceConfigurer) {
      return Fluently
        .Configure()
        .Database(persistenceConfigurer)
        .Cache(ConfigureCache)
        .Mappings(m => m.AutoMappings.Add(
          AutoMap
            .AssemblyOf<Muwesome.DomainModel.Identifiable>(new DatabaseMappingConfiguration())
            .Conventions.Setup(ConfigureConventions)
            .UseOverridesFromAssembly(Assembly.GetExecutingAssembly())))
        /*.ExposeConfiguration(c => c.SetProperty("generate_statistics", "true"))*/
        .BuildConfiguration();

      void ConfigureCache(CacheSettingsBuilder cacheBuilder) =>
        cacheBuilder.ProviderClass<HashtableCacheProvider>().UseQueryCache().UseSecondLevelCache();

      void ConfigureConventions(IConventionFinder conventions) {
        conventions.Add(ForeignKey.EndsWith("Id"));
        conventions.Add(new NotNullConvention());
        conventions.Add(new NotLazyLoadConvention());
        conventions.Add(new CacheConfigurationConvention());
      }
    }

    private IPersistenceConfigurer CreatePersistenceConfigurer(StorageEngine storageEngine, string connectionString) {
      switch (storageEngine) {
        case StorageEngine.InMemory:
          return SQLiteConfiguration.Standard.InMemory().Provider<InMemoryConnectionProvider>();
        case StorageEngine.Sqlite:
          return SQLiteConfiguration.Standard.ConnectionString(connectionString);
        default:
          throw new InvalidEnumArgumentException(nameof(storageEngine), (int)storageEngine, typeof(StorageEngine));
      }
    }
  }
}