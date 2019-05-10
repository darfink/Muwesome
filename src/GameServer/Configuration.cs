using System;
using System.Text;
using Muwesome.Persistence.EntityFramework;
using Muwesome.Protocol.Game;

namespace Muwesome.GameServer {
  /// <summary>The game server configuration.</summary>
  public class Configuration {
    /// <summary>Gets or sets the server code.</summary>
    public ushort ServerCode { get; set; } = 0;

    /// <summary>Gets or sets the client listener address.</summary>
    public GameServerEndPoint ClientListenerEndPoint { get; set; } = new GameServerEndPoint();

    /// <summary>Gets or sets the default client version.</summary>
    public ClientVersion DefaultClientVersion { get; set; } = ClientVersion.V10203;

    /// <summary>Gets or sets the default client serial.</summary>
    public byte[] DefaultClientSerial { get; set; } = Encoding.ASCII.GetBytes("ugkgameshield000");

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

    /// <summary>Gets or sets the connect server RPC host.</summary>
    public string ConnectServerGrpcHost { get; set; } = "127.0.0.1";

    /// <summary>Gets or sets the connect server RPC port.</summary>
    public ushort ConnectServerGrpcPort { get; set; } = 22336;

    /// <summary>Gets or sets the login server RPC host.</summary>
    public string LoginServerGrpcHost { get; set; } = "127.0.0.1";

    /// <summary>Gets or sets the login server RPC port.</summary>
    public ushort LoginServerGrpcPort { get; set; } = 22337;

    /// <summary>Gets or sets the persistence configuration.</summary>
    public PersistenceConfiguration PersistenceConfiguration { get; set; } = PersistenceConfiguration.InMemory();
  }
}