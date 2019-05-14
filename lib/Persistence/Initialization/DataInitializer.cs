using Muwesome.DomainModel.Configuration;
using Muwesome.DomainModel.Entities;
using static BCrypt.Net.BCrypt;

namespace Muwesome.Persistence.Initialization {
  public class DataInitializer {
    private readonly IPersistenceContextProvider persistenceContextProvider;

    /// <summary>Initializes a new instance of the <see cref="DataInitializer"/> class.</summary>
    public DataInitializer(IPersistenceContextProvider persistenceContextProvider) =>
      this.persistenceContextProvider = persistenceContextProvider;

    /// <summary>Creates all initial persistence data.</summary>
    public void CreateInitialData() {
      using (var context = this.persistenceContextProvider.CreateContext()) {
        this.CreateCharacterClasses(context);
        this.CreateTestAccounts(context, count: 10);
        context.SaveChanges();
      }
    }

    private void CreateTestAccounts(IContext context, int count) {
      for (int i = 1; i <= count; i++) {
        var account = context.Create<Account>($"test{i}", HashPassword("password"), "123456789");
        //account.Characters.Add(this.CreateCharacter(context, $"Foobar{i}", 0));
      }
    }

    /*private Character CreateCharacter(IContext context, string name, byte slot) {
      var character = context.Create<Character>(name, slot);
      return character;
    }*/

    private void CreateCharacterClasses(IContext context) {
      var characterClass = context.Create<CharacterClass>("Dark Wizard");
      characterClass.PointsPerLevel = 5;
    }
  }
}