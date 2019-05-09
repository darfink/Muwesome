using System;

namespace Muwesome.DomainModel.Entities {
  /// <summary>The account of a user.</summary>
  public class Account {
    /// <summary>Gets or sets the ID.</summary>
    public Guid Id { get; set; }

    /// <summary>Gets or sets the username.</summary>
    public string Username { get; set; }

    /// <summary>Gets or sets the password hash.</summary>
    public string PasswordHash { get; set; }

    /// <summary>Gets or sets the E-mail.</summary>
    public string Mail { get; set; }

    /// <summary>Gets or sets the security code.</summary>
    public string SecurityCode { get; set; }

    /// <summary>Gets or sets the registration date.</summary>
    public DateTime RegistrationDate { get; set; }

    /// <summary>Gets or sets the last login time.</summary>
    public DateTime? LastLoginTime { get; set; }

    /// <summary>Gets or sets the last failed login time.</summary>
    public DateTime? LastFailedLoginTime { get; set; }

    /// <summary>Gets or sets the number of consecutively failed login attempts.</summary>
    public int ConsecutiveFailedLoginAttempts { get; set; }
  }
}