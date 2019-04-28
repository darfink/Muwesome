using System;
using Muwesome.Packet.Utility;
using Muwesome.Protocol;
using Muwesome.Protocol.Connect.V20050502;
using Muwesome.Network;

namespace Muwesome.ConnectServer.PacketHandlers {
  internal class GameServerInfoRequestHandler : IPacketHandler<Client> {
    private readonly IGameServerController _gameServerController;

    /// <summary>Creates a new <see cref="GameServerInfoRequestHandler" />.</summary>
    public GameServerInfoRequestHandler(IGameServerController gameServerController) =>
      _gameServerController = gameServerController;

    /// <inheritdoc />
    public bool HandlePacket(Client client, Span<byte> packet) {
      ref var request = ref PacketHelper.ParsePacket<GameServerInfoRequest>(packet);
      var selectedServer = _gameServerController.GetServerByCode(request.ServerCode);

      if (selectedServer is null || selectedServer.IsFull) {
        using (var writer = client.Connection.SendPacket<GameServerUnavailable>()) {
          writer.Packet.ServerCode = request.ServerCode;
        }
      } else {
        using (var writer = client.Connection.SendPacket<GameServerInfo>()) {
          ref var server = ref writer.Packet;
          server.Host = selectedServer.Host;
          server.Port = selectedServer.Port;
        }
      }

      return true;
    }
  }
}