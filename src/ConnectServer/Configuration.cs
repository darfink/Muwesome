using System;
using System.Net;

namespace Muwesome.ConnectServer {
  public class Configuration {
    /// <summary>Gets or sets the client listener address.</summary>
    public IPEndPoint ClientListenerEndPoint { get; set; } = new IPEndPoint(IPAddress.Any, 2004);

    /// <summary>Gets or sets the maximum client idle time until being disconnected.</summary>
    public TimeSpan MaxIdleTime { get; set; } = TimeSpan.FromMinutes(1);

    /// <summary>Gets or sets the maximum allowed client packet size.</summary>
    public int MaxPacketSize { get; set; } = 6;

    /// <summary>Gets or sets the server's maximum connections.</summary>
    public int MaxConnections { get; set; } = 10_000;

    /// <summary>Gets or sets the maximum connections per IP address.</summary>
    public int MaxConnectionsPerIp { get; set; } = 50;

    /// <summary>Gets or sets a value indicating whether clients are disconnected when sending unknown packets.</summary>
    public bool DisconnectOnUnknownPacket { get; set; } = true;

    /// <summary>Gets or sets the gRPC host.</summary>
    public string GrpcServiceHost { get; set; } = "127.0.0.1";

    /// <summary>Gets or sets the gRPC port.</summary>
    public ushort GrpcServicePort { get; set; } = 22336;

    // TODO: encryption/decryption settings
  }
}