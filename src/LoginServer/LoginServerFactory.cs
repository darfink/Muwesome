using Muwesome.Persistence;

namespace Muwesome.LoginServer {
  public static class LoginServerFactory {
    /// <summary>Initializes a new instance of the <see cref="LoginServer" /> class with default implementations.</summary>
    public static LoginServer Create(Configuration config, IPersistenceContextProvider persistenceContextProvider) =>
      new LoginServer(config, persistenceContextProvider);
  }
}