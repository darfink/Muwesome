using System;

namespace Muwesome.GameServer {
  /// <summary>The game server configuration.</summary>
  public class Configuration {
    /// <summary>Gets or sets the maximum client idle time until being disconnected.</summary>
    public TimeSpan MaxIdleTime { get; set; } = TimeSpan.FromMinutes(1);

    /// <summary>Gets or sets the maximum allowed client packet size.</summary>
    public int MaxPacketSize { get; set; } = 1024;

    /// <summary>Gets or sets the server's maximum connections.</summary>
    public int MaxConnections { get; set; } = 1000;

    /// <summary>Gets or sets the maximum connections per IP address.</summary>
    public int MaxConnectionsPerIp { get; set; } = 50;

    /// <summary>Gets or sets a value indicating whether clients are disconnected when sending unknown packets.</summary>
    public bool DisconnectOnUnknownPacket { get; set; } = false;
  }
}