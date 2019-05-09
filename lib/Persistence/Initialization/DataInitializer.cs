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
        this.CreateTestAccounts(context, count: 10);
        context.SaveChanges();
      }
    }

    private void CreateTestAccounts(IContext context, int count) {
      for (int i = 1; i <= count; i++) {
        var login = context.Create<Account>();
        login.Username = $"test{i}";
        login.PasswordHash = HashPassword("password");
        login.SecurityCode = "123456789";
      }
    }
  }
}