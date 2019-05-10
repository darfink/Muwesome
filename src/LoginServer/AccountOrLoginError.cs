using System;
using Muwesome.DomainModel.Entities;
using OneOf;

namespace Muwesome.LoginServer {
  /// <summary>Represents all possible login errors.</summary>
  internal enum LoginError {
    AlreadyConnected,
    InvalidAccount,
    InvalidPassword,
    LockedOut,
  }

  /// <summary>Represents the result of a login request.</summary>
  internal class AccountOrLoginError : OneOfBase<Account, LoginError> {
    /// <summary>Gets a value indicating whether the login was successful or not.</summary>
    public bool Success => this.IsT0;

    /// <summary>Converts a <see cref="LoginError" /> to <see cref="AccountOrLoginError" />.</summary>
    public static implicit operator AccountOrLoginError(LoginError error) => (AccountOrLoginError)error;

    /// <summary>Converts an <see cref="Account" /> to <see cref="AccountOrLoginError" />.</summary>
    public static implicit operator AccountOrLoginError(Account account) => (AccountOrLoginError)account;
  }
}