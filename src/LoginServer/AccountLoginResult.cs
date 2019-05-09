using System;
using Muwesome.DomainModel.Entities;

namespace Muwesome.LoginServer {
  /// <summary>Represents all possible login result types.</summary>
  internal enum AccountLoginResultType {
    Success,
    AlreadyConnected,
    InvalidAccount,
    InvalidPassword,
    LockedOut,
  }

  /// <summary>Represents the result of an account request.</summary>
  internal class AccountLoginResult {
    /// <summary>Gets or sets the account entity.</summary>
    public Account Account { get; set; }

    /// <summary>Gets or sets the result type.</summary>
    public AccountLoginResultType Type { get; set; }

    /// <inheritdoc />
    public override string ToString() =>
      Enum.GetName(typeof(AccountLoginResultType), this.Type);
  }
}