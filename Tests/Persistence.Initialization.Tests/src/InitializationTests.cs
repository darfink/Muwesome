using Muwesome.Persistence.Initialization;
using Muwesome.Persistence.EntityFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Muwesome.Persistence.Initialization.Tests {
  [TestClass]
  public class InitializationTests {
    [TestMethod]
    public void Initialize_With_Sqlite() {
      var config = new ConnectionConfiguration {
        StorageEngine = StorageEngine.Sqlite,
        ConnectionString = "Data Source=Sqlite_Init_Test.db",
        ContextType = null,
      };

      var persistenceContextProvider = new PersistenceContextProvider(config);
      persistenceContextProvider.RecreateStorage();

      var dataInitializer = new DataInitializer(persistenceContextProvider);
      dataInitializer.CreateInitialData();
    }
  }
}