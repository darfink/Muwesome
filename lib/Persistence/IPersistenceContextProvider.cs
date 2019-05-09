using System;

namespace Muwesome.Persistence {
  public interface IPersistenceContextProvider : IDisposable {
    /// <summary>Creates a new persistence context.</summary>
    IContext CreateContext();

    /// <summary>Creates a new account context.</summary>
    IAccountContext CreateAccountContext();
  }
}