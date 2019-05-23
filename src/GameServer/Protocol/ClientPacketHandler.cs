using System;
using System.Threading.Tasks;
using log4net;
using Muwesome.GameServer.Protocol.Handlers;
using Muwesome.Network;
using Muwesome.Packet;
using Muwesome.Packet.Utility;
using Muwesome.Protocol;

namespace Muwesome.GameServer.Protocol {
  /// <summary>A client packet handler.</summary>
  internal class ClientPacketHandler : ConfigurablePacketHandler<Client> {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(ClientPacketHandler));

    /// <summary>Gets or sets a value indicating whether client's are disconnected when sending unknown packets.</summary>
    public bool DisconnectOnUnknownPacket { get; set; }

    /// <inheritdoc />
    public override bool HandlePacket(Client client, Span<byte> packet) {
      bool packetWasHandled = base.HandlePacket(client, packet);

      if (!packetWasHandled) {
        Logger.DebugFormat("Received an unhandled packet: {0}", packet.ToHexString());
        if (this.DisconnectOnUnknownPacket) {
          Logger.InfoFormat("Disconnecting client {0}; received an unknown packet", client);
          client.Connection.Disconnect();
        }
      } else {
        Logger.DebugFormat("Received handled packet: {0}", packet.ToHexString());
      }

      return packetWasHandled;
    }

    /// <summary>Registers a new handler for a packet.</summary>
    // TODO: This should only be called during ctor
    public void RegisterHandler<TPacket>(PacketHandler<TPacket> packetHandler)
        where TPacket : IPacket => this.RegisterHandler(packetHandler.Identifier, packetHandler);
  }
}