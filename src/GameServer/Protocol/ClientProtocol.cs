using Muwesome.Protocol;
using Muwesome.Protocol.Game;

namespace Muwesome.GameServer.Protocol {
  /// <summary>A client protocol configuration.</summary>
  internal class ClientProtocol {
    /// <summary>Initializes a new instance of the <see cref="ClientProtocol"/> class.</summary>
    public ClientProtocol(ClientVersion version, ClientPacketHandler packetHandler, ClientPacketDispatcher packetDispatcher) {
      this.Version = version;
      this.PacketDispatcher = packetDispatcher;
      this.PacketHandler = packetHandler;
    }

    /// <summary>Gets the protocol client version.</summary>
    public ClientVersion Version { get; }

    /// <summary>Gets the packet dispatcher.</summary>
    public ClientPacketDispatcher PacketDispatcher { get; }

    /// <summary>Gets the packet handler.</summary>
    public ClientPacketHandler PacketHandler { get; }
  }
}