using System;
using System.Net;

namespace Muwesome.LoginServer {
  public class Configuration {
    /// <summary>Gets or sets the gRPC host.</summary>
    public string GrpcListenerHost { get; set; } = "127.0.0.1";

    /// <summary>Gets or sets the gRPC port.</summary>
    public ushort GrpcListenerPort { get; set; } = 22337;
  }
}