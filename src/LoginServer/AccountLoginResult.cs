using Muwesome.DomainModel.Entities;

namespace Muwesome.LoginServer {
  /// <summary>Represents the result of an account request.</summary>
  internal class AccountLoginResult {
    /// <summary>Gets or sets the account entity.</summary>
    public Account Account { get; set; }

    /// <summary>Gets a value indicating whether the request was successful.</summary>
    public bool Success =>
      this.Account != null &&
      !this.AlreadyConnected &&
      !this.InvalidAccount &&
      !this.InvalidPassword &&
      !this.TimedOut;

    /// <summary>Gets or sets a value indicating whether the account is already connected.</summary>
    public bool AlreadyConnected { get; set; }

    /// <summary>Gets or sets a value indicating whether the account username is invalid.</summary>
    public bool InvalidAccount { get; set; }

    /// <summary>Gets or sets a value indicating whether the account password is invalid.</summary>
    public bool InvalidPassword { get; set; }

    /// <summary>Gets or sets a value indicating whether the account is timed out or not.</summary>
    public bool TimedOut { get; set; }
  }
}