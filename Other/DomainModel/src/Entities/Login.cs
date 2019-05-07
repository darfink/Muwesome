using System;

namespace Muwesome.DomainModel.Entities {
  /// <summary>The login information of an account.</summary>
  public class Login {
    /// <summary>Gets or sets the ID.</summary>
    public Guid Id { get; set; }

    /// <summary>Gets or sets the username.</summary>
    public string Username { get; set; }

    /// <summary>Gets or sets the password hash.</summary>
    public string PasswordHash { get; set; }

    /// <summary>Gets or sets the last login time.</summary>
    public DateTime LastLoginTime { get; set; }

    /// <summary>Gets or sets the last failed login time.</summary>
    public DateTime LastFailedLoginTime { get; set; }

    /// <summary>Gets or sets the number of consecutively failed login attempts.</summary>
    public int FailedLoginAttempts { get; set; }
  }
}