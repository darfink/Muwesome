using System;
using Muwesome.Protocol;

namespace Muwesome.ConnectServer.PacketHandlers {
  internal class ClientUpdateRequestHandler : IPacketHandler<Client> {
    /// <inheritdoc />
    public bool HandlePacket(Client client, Span<byte> packet) {
      // TODO: Implement (although the client does not seem receptible to any response)
      return true;
    }
  }
}