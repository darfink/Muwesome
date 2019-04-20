using System;
using Muwesome.Protocol;

namespace Muwesome.ConnectServer {
  internal class ClientProtocolHandler : ConfigurablePacketHandler<Client> {
    // TODO: SEND HELLO TO DIS FFKN CLIENT
    public ClientProtocolHandler() {
    }

    /// <summary>Gets or sets whether client's are disconnected when sending unknown packets.</summary>
    public bool DisconnectOnUnknownPacket { get; set; }

    /// <inheritdoc />
    public override bool HandlePacket(Client client, Span<byte> packet) {
      bool packetWasHandled = base.HandlePacket(client, packet);

      if (!packetWasHandled) {
        // TODO: LOOOG HERRREEEE!!!!!
        if (DisconnectOnUnknownPacket) {
          // TODO: AND LOOGG GHEERERE
          client.Connection.Disconnect();
        }
      }

      return packetWasHandled;
    }
  }
}