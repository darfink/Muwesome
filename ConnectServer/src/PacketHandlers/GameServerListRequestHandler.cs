using System;
using System.Collections.Generic;
using System.Linq;
using Muwesome.Network;
using Muwesome.Packet.Utility;
using Muwesome.Protocol;
using Muwesome.Protocol.Connect.V20050502;

namespace Muwesome.ConnectServer.PacketHandlers {
  internal class GameServerListRequestHandler : IPacketHandler<Client> {
    private readonly IGameServerController gameServerController;
    private byte[] gameServerListPacket = Array.Empty<byte>();
    private int gameServerListPacketSize;

    /// <summary>Initializes a new instance of the <see cref="GameServerListRequestHandler"/> class.</summary>
    public GameServerListRequestHandler(IGameServerController gameServerController) {
      this.gameServerController = gameServerController;
      this.gameServerController.GameServerRegistered += (_, ev) => this.OnGameServerChange();
      this.gameServerController.GameServerDeregistered += (_, ev) => this.OnGameServerChange();
      this.gameServerController.GameServerUpdated += (_, ev) => this.OnGameServerChange();
    }

    /// <inheritdoc />
    public bool HandlePacket(Client client, Span<byte> packet) {
      if (this.gameServerListPacketSize == 0) {
        this.CreateServerListPacket(this.gameServerController.Servers);
      }

      using (var writer = client.Connection.StartWrite(this.gameServerListPacketSize)) {
        this.gameServerListPacket.AsSpan().Slice(0, this.gameServerListPacketSize).CopyTo(writer.Span);
      }

      return true;
    }

    private void CreateServerListPacket(IReadOnlyCollection<GameServerEntry> servers) {
      this.gameServerListPacketSize = PacketHelper.GetPacketSize<GameServerList, GameServerList.GameServer>(servers.Count);

      if (this.gameServerListPacket.Length < this.gameServerListPacketSize) {
        this.gameServerListPacket = new byte[this.gameServerListPacketSize];
      }

      PacketHelper.CreatePacket<GameServerList, GameServerList.GameServer>(
        servers.Count,
        this.gameServerListPacket.AsSpan(),
        out Span<GameServerList.GameServer> serverEntries);

      foreach (var (index, server) in servers.Select((s, i) => (i, s))) {
        serverEntries[index].Code = server.Code;
        serverEntries[index].Load = server.Load;
      }
    }

    private void OnGameServerChange() => this.gameServerListPacketSize = 0;
  }
}