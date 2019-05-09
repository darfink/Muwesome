using System;
using System.Net;
using Muwesome.Persistence.EntityFramework;

namespace Muwesome.LoginServer {
  public class Configuration {
    /// <summary>Gets or sets the gRPC host.</summary>
    public string GrpcServiceHost { get; set; } = "127.0.0.1";

    /// <summary>Gets or sets the gRPC port.</summary>
    public ushort GrpcServicePort { get; set; } = 22337;

    /// <summary>Gets or sets the number of failed login attempts until a timeout takes effect.</summary>
    public int FailedLoginAttemptsUntilAccountLockOut { get; set; } = 1;

    /// <summary>Gets or sets the maximum account time out.</summary>
    public TimeSpan MaxAccountLockOut { get; set; } = TimeSpan.FromDays(2);

    /// <summary>Gets or sets the database configuration.</summary>
    public ConnectionConfiguration Database { get; set; } = new ConnectionConfiguration();
  }
}