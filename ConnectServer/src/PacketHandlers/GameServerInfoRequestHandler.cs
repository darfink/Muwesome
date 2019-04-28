using System;
using Muwesome.Protocol;
using Muwesome.Protocol.Connect.V20050502;
using Muwesome.Network;

namespace Muwesome.ConnectServer.PacketHandlers {
  internal class GameServerInfoRequestHandler : IPacketHandler<Client> {
    private readonly IGameServerController _gameServerController;

    public GameServerInfoRequestHandler(IGameServerController gameServerController) =>
      _gameServerController = gameServerController;

    /// <inheritdoc />
    public bool HandlePacket(Client client, Span<byte> packet) {
      ref var request = ref ProtocolHelper.ParsePacket<GameServerInfoRequest>(packet);
      var selectedServer = _gameServerController.GetServerByCode(request.ServerCode);

      if (selectedServer is null || selectedServer.IsFull) {
        int size = ProtocolHelper.GetPacketSize<GameServerUnavailable>();
        using (var writer = client.Connection.StartWrite(size)) {
          ref var unavailable = ref ProtocolHelper.CreatePacket<GameServerUnavailable>(writer.Span);
          unavailable.ServerCode = request.ServerCode;
        }
      } else {
        int size = ProtocolHelper.GetPacketSize<GameServerInfo>();
        using (var writer = client.Connection.StartWrite(size)) {
          ref var server = ref ProtocolHelper.CreatePacket<GameServerInfo>(writer.Span);
          server.Host = selectedServer.Host;
          server.Port = selectedServer.Port;
        }
      }

      return true;
    }
  }
}