using System;
using System.Net;

namespace Muwesome.LoginServer {
  public class Configuration {
    /// <summary>Gets or sets the number of failed login attempts until a timeout takes effect.</summary>
    public int FailedLoginAttemptsUntilAccountLockOut { get; set; } = 1;

    /// <summary>Gets or sets the maximum account time out.</summary>
    public TimeSpan MaxAccountLockOut { get; set; } = TimeSpan.FromDays(2);
  }
}