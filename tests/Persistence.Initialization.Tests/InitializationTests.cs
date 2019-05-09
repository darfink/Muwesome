using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Muwesome.DomainModel.Entities;
using Muwesome.Persistence.EntityFramework;
using Muwesome.Persistence.Initialization;

namespace Muwesome.Persistence.Initialization.Tests {
  [TestClass]
  public class InitializationTests {
    [TestMethod]
    public void Initialized_In_Memory_Data_Is_Persistent() {
      var config = PersistenceConfiguration.InMemory();
      this.ValidateDataPersistance(new PersistenceContextProvider(config));
    }

    [TestMethod]
    public void Initialized_Sqlite_Data_Is_Persistent() {
      var config = new PersistenceConfiguration() {
        ConnectionString = $"Data Source={nameof(this.Initialized_Sqlite_Data_Is_Persistent)}.db",
        StorageEngine = StorageEngine.Sqlite,
      };

      this.ValidateDataPersistance(new PersistenceContextProvider(config));
    }

    private void ValidateDataPersistance(PersistenceContextProvider persistenceContextProvider) {
      using (persistenceContextProvider) {
        persistenceContextProvider.RecreateStorage();

        var dataInitializer = new DataInitializer(persistenceContextProvider);
        dataInitializer.CreateInitialData();

        // Execute this twice to ensure the data is persistent between contexts
        for (int i = 0; i < 2; i++) {
          using (var context = persistenceContextProvider.CreateContext()) {
            Assert.AreEqual(context.GetAll<Account>().Count(), 10);
          }
        }
      }
    }
  }
}