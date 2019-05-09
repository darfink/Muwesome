using Muwesome.LoginServer.Services;
using Muwesome.Persistence;
using Muwesome.Persistence.EntityFramework;

namespace Muwesome.LoginServer {
  public static class LoginServerFactory {
    /// <summary>Initializes a new instance of the <see cref="LoginServer" /> class with default implementations.</summary>
    public static LoginServer Create(Configuration config) {
      var persistenceContextProvider = new PersistenceContextProvider(config.Database);
      var accountController = new AccountController(config, persistenceContextProvider);
      var serviceController = ServiceControllerFactory.Create(config, accountController);
      return new LoginServer(serviceController);
    }
  }
}