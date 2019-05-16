using System;
using System.Collections.Generic;

namespace Muwesome.DomainModel.Entities {
  /// <summary>The account of a user.</summary>
  public class Account : Identifiable {
    /// <summary>Initializes a new instance of the <see cref="Account"/> class.</summary>
    public Account(string username, string passwordHash, string securityCode) {
      this.Username = username;
      this.PasswordHash = passwordHash;
      this.SecurityCode = securityCode;
      this.Characters = new List<Character>();
      this.RegistrationDate = DateTime.UtcNow;
    }

    /// <summary>Initializes a new instance of the <see cref="Account"/> class.</summary>
    protected Account() {
    }

    /// <summary>Gets or sets the username.</summary>
    public string Username { get; set; }

    /// <summary>Gets or sets the password hash.</summary>
    public string PasswordHash { get; set; }

    /// <summary>Gets or sets the E-mail.</summary>
    public string Mail { get; set; }

    /// <summary>Gets or sets the security code.</summary>
    public string SecurityCode { get; set; }

    /// <summary>Gets or sets the characters.</summary>
    public IList<Character> Characters { get; set; }

    /// <summary>Gets or sets the vault.</summary>
    public ItemStorage Vault { get; set; }

    /// <summary>Gets or sets the registration date.</summary>
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

    /// <summary>Gets or sets the last login time.</summary>
    public DateTime? LastLoginTime { get; set; }

    /// <summary>Gets or sets the last failed login time.</summary>
    public DateTime? LastFailedLoginTime { get; set; }

    /// <summary>Gets or sets the number of consecutively failed login attempts.</summary>
    public int ConsecutiveFailedLoginAttempts { get; set; }
  }
}