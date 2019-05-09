using System;
using System.Threading.Tasks;
using log4net;
using Muwesome.Network;
using Muwesome.Packet;
using Muwesome.Packet.Utility;
using Muwesome.Protocol;

namespace Muwesome.GameServer.Protocol {
  /// <summary>A client packet handler.</summary>
  public class ClientPacketHandler : ConfigurablePacketHandler<Client> {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(ClientPacketHandler));

    /// <summary>Gets or sets a value indicating whether client's are disconnected when sending unknown packets.</summary>
    public bool DisconnectOnUnknownPacket { get; set; }

    /// <inheritdoc />
    public override bool HandlePacket(Client client, Span<byte> packet) {
      bool packetWasHandled = base.HandlePacket(client, packet);

      if (!packetWasHandled) {
        Logger.Debug($"Received an unhandled packet: {packet.ToHexString()}");
        if (this.DisconnectOnUnknownPacket) {
          Logger.Info($"Disconnecting client {client}; received an unknown packet");
          client.Connection.Disconnect();
        }
      }

      return packetWasHandled;
    }
  }
}