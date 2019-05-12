using Muwesome.DomainModel.Entities;
using OneOf;

namespace Muwesome.GameLogic {
  /// <summary>Represents an account or login result.</summary>
  public class AccountOrLoginResult : OneOfBase<Account, Actions.LoginResult> {
    /// <summary>Converts a <see cref="Actions.LoginResult" /> to <see cref="AccountOrLoginResult" />.</summary>
    public static implicit operator AccountOrLoginResult(Actions.LoginResult error) => (AccountOrLoginResult)error;

    /// <summary>Converts an <see cref="Account" /> to <see cref="AccountOrLoginResult" />.</summary>
    public static implicit operator AccountOrLoginResult(Account account) => (AccountOrLoginResult)account;
  }
}