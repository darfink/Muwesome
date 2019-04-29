using System;

namespace Muwesome.DomainModel.Entities {
  /// <summary>The account of a user.</summary>
  public class Account {
    /// <summary>Gets or sets the ID.</summary>
    public Guid Id { get; set; }

    /// <summary>Gets or sets the E-mail.</summary>
    public string Mail { get; set; }

    /// <summary>Gets or sets the security code.</summary>
    public string SecurityCode { get; set; }

    /// <summary>Gets or sets the registration date.</summary>
    public DateTime RegistrationDate { get; set; }
  }
}
