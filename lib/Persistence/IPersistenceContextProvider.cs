namespace Muwesome.Persistence {
  public interface IPersistenceContextProvider {
    /// <summary>Creates a new persistence context.</summary>
    IContext CreateContext();

    /// <summary>Creates a new account context.</summary>
    IAccountContext CreateAccountContext();
  }
}