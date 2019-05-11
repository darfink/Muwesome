using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Muwesome.Interfaces {
  /// <summary>Represents all possible login errors.</summary>
  public enum LoginError {
    InvalidAccount,
    InvalidPassword,
    AccountIsBlocked,
    AccountIsLockedOut,
    AccountIsAlreadyConnected,
  }

  /// <summary>An account login service.</summary>
  /// <remarks>This is implemented by the <see cref="LoginServer" />.</remarks>
  public interface IAccountLoginService {
    /// <summary>Gets the number of logged in accounts.</summary>
    int AccountsLoggedIn { get; }

    /// <summary>Attempts to login an account using the provided credentials.</summary>
    Task<LoginError?> TryLoginAsync(string username, string password);

    /// <summary>Attempts to log out an account by its username.</summary>
    Task<bool> TryLogoutAsync(string username);
  }
}