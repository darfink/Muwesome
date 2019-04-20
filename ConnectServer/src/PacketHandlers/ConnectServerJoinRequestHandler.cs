using System;
using Muwesome.Protocol;

namespace Muwesome.ConnectServer.PacketHandlers {
  internal class ConnectServerJoinRequestHandler : IPacketHandler<Client> {
    /// <inheritdoc />
    public bool HandlePacket(Client client, Span<byte> packet) {
      throw new NotImplementedException();
    }
  }
}