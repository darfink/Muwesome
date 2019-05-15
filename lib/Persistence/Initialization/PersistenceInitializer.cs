using Muwesome.DomainModel.Components;
using Muwesome.DomainModel.Configuration;
using Muwesome.DomainModel.Entities;
using static BCrypt.Net.BCrypt;

namespace Muwesome.Persistence.Initialization {
  public class PersistenceInitializer {
    private readonly IPersistenceContextProvider persistenceContextProvider;

    /// <summary>Initializes a new instance of the <see cref="PersistenceInitializer"/> class.</summary>
    public PersistenceInitializer(IPersistenceContextProvider persistenceContextProvider) =>
      this.persistenceContextProvider = persistenceContextProvider;

    /// <summary>Creates the persistent configuration.</summary>
    public void CreateConfiguration() =>
      new ConfigInitializer(this.persistenceContextProvider).Initalize();

    /// <summary>Creates all persistent test data.</summary>
    public void CreateTestData() {
      new TestDataInitializer(this.persistenceContextProvider).Initialize();
    }
  }
}