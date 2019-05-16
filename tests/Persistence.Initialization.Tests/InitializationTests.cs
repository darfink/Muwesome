using System.Linq;
using System.Reflection;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Muwesome.DomainModel.Configuration;
using Muwesome.DomainModel.Entities;
using Muwesome.Persistence.Initialization;
using Muwesome.Persistence.NHibernate;

namespace Muwesome.Persistence.Initialization.Tests {
  [TestClass]
  public class InitializationTests {
    [TestMethod]
    public void Initialized_In_Memory_Data_Is_Persistent() {
      var config = PersistenceConfiguration.InMemory();
      this.ValidatePersistenceCache(new PersistenceContextProvider(config));
    }

    [TestMethod]
    public void Initialized_Sqlite_Data_Is_Persistent() {
      var repository = LogManager.GetRepository(Assembly.GetEntryAssembly());
      log4net.Config.BasicConfigurator.Configure(repository);

      var config = new PersistenceConfiguration(StorageEngine.Sqlite) {
        ConnectionString = $"Data Source={nameof(this.Initialized_Sqlite_Data_Is_Persistent)}.db;",
        SchemaOperation = SchemaOperation.Recreate,
      };

      this.ValidatePersistenceCache(new PersistenceContextProvider(config));

      config.SchemaOperation = SchemaOperation.None;
      this.ValidateEagerLoadingWithColdCache(new PersistenceContextProvider(config));
    }

    private void ValidatePersistenceCache(PersistenceContextProvider persistenceContextProvider) {
      using (persistenceContextProvider) {
        var dataInitializer = new PersistenceInitializer(persistenceContextProvider);
        dataInitializer.CreateConfiguration();
        dataInitializer.CreateTestData();

        GameConfiguration config1 = null;
        using (var context = persistenceContextProvider.CreateContext()) {
          config1 = context.GetAll<GameConfiguration>().First();
        }

        GameConfiguration config2 = null;
        using (var context = persistenceContextProvider.CreateContext()) {
          config2 = context.GetAll<GameConfiguration>().First();
        }

        // These references should be the same (fetched from the configuration cache)
        Assert.AreSame(config1.MapDefinitions[0].Name, config2.MapDefinitions[0].Name);
      }
    }

    private void ValidateEagerLoadingWithColdCache(PersistenceContextProvider persistenceContextProvider) {
      using (persistenceContextProvider) {
        using (var context = persistenceContextProvider.CreateContext()) {
          var account = context.GetAll<Account>().First();
          Assert.IsNotNull(account.Characters[0].CurrentMap.Name);
        }
      }
    }
  }
}