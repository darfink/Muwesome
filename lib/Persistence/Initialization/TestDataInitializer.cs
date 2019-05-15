using System.Linq;
using Muwesome.DomainModel.Components;
using Muwesome.DomainModel.Configuration;
using Muwesome.DomainModel.Entities;
using static BCrypt.Net.BCrypt;

namespace Muwesome.Persistence.Initialization {
  public class TestDataInitializer {
    private readonly IPersistenceContextProvider persistenceContextProvider;
    private GameConfiguration gameConfiguration;
    private IContext context;

    /// <summary>Initializes a new instance of the <see cref="TestDataInitializer"/> class.</summary>
    public TestDataInitializer(IPersistenceContextProvider persistenceContextProvider) =>
      this.persistenceContextProvider = persistenceContextProvider;

    /// <summary>Creates all initial persistence data.</summary>
    public void Initialize() {
      using (this.context = this.persistenceContextProvider.CreateContext()) {
        this.gameConfiguration = this.context.GetAll<GameConfiguration>().First();
        this.CreateTestAccounts(count: 10);
        this.context.SaveChanges();
      }
    }

    private void CreateTestAccounts(int count) {
      for (int i = 1; i <= count; i++) {
        var account = this.context.Create<Account>($"test{i}", HashPassword("password"), "123456789");
        account.Characters.Add(this.CreateCharacter($"Foobar{i}", slot: 0));
      }
    }

    private Character CreateCharacter(string name, byte slot) {
      var @class = this.gameConfiguration.CharacterClasses.First();
      var inventory = this.context.Create<ItemStorage>((byte)8, (byte)8);
      return new Character(name, @class, inventory) {
        Slot = slot,
      };
    }
  }
}