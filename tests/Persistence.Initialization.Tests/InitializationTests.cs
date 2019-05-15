using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Muwesome.DomainModel.Entities;
using Muwesome.Persistence.Initialization;
using Muwesome.Persistence.NHibernate;

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
      var config = new PersistenceConfiguration(StorageEngine.Sqlite) {
        ConnectionString = $"Data Source={nameof(this.Initialized_Sqlite_Data_Is_Persistent)}.db;",
        SchemaOperation = SchemaOperation.Recreate,
      };

      this.ValidateDataPersistance(new PersistenceContextProvider(config));
    }

    private void ValidateDataPersistance(PersistenceContextProvider persistenceContextProvider) {
      using (persistenceContextProvider) {
        var dataInitializer = new PersistenceInitializer(persistenceContextProvider);
        dataInitializer.CreateConfiguration();
        dataInitializer.CreateTestData();

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