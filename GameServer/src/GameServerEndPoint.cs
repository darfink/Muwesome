using System.Net;
using Muwesome.Protocol.Game;

namespace Muwesome.GameServer {
  /// <summary>A game server IP/host end point.</summary>
  public class GameServerEndPoint : IPEndPoint {
    /// <summary>Initializes a new instance of the <see cref="GameServerEndPoint" /> class.</summary>
    public GameServerEndPoint()
        : base(IPAddress.Any, 0) {
    }

    /// <summary>Gets or sets the server's host as shown to clients.</summary>
    public string ExternalHost { get; set; } = null;

    /// <summary>Gets or sets the server's port as shown to clients.</summary>
    public ushort? ExternalPort { get; set; } = null;

    /// <summary>Gets or sets the end points associated client version.</summary>
    public ClientVersion? ClientVersion { get; set; } = null;

    /// <summary>Gets or sets the end points associated client serial.</summary>
    public byte[] ClientSerial { get; set; } = null;
  }
}