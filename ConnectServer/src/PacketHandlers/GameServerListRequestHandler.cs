using System;
using Muwesome.Protocol;
using Muwesome.Protocol.Connect.V20050502;

namespace Muwesome.ConnectServer.PacketHandlers {
  internal class GameServerListRequestHandler : IPacketHandler<Client> {
    /// <inheritdoc />
    public bool HandlePacket(Client client, Span<byte> packet) {
      throw new NotImplementedException();
    }
  }
}