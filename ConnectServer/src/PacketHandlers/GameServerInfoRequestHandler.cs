using System;
using Muwesome.Network;
using Muwesome.Packet.Utility;
using Muwesome.Protocol;
using Muwesome.Protocol.Connect.V20050502;

namespace Muwesome.ConnectServer.PacketHandlers {
  internal class GameServerInfoRequestHandler : IPacketHandler<Client> {
    private readonly IGameServerController gameServerController;

    /// <summary>Initializes a new instance of the <see cref="GameServerInfoRequestHandler"/> class.</summary>
    public GameServerInfoRequestHandler(IGameServerController gameServerController) =>
      this.gameServerController = gameServerController;

    /// <inheritdoc />
    public bool HandlePacket(Client client, Span<byte> packet) {
      ref var request = ref PacketHelper.ParsePacket<GameServerInfoRequest>(packet);
      var selectedServer = this.gameServerController.GetServerByCode(request.ServerCode);

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