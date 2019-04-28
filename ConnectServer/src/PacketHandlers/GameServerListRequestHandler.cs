using System;
using System.Collections.Generic;
using System.Linq;
using Muwesome.Network;
using Muwesome.Packet.Utility;
using Muwesome.Protocol;
using Muwesome.Protocol.Connect.V20050502;

namespace Muwesome.ConnectServer.PacketHandlers {
  internal class GameServerListRequestHandler : IPacketHandler<Client> {
    private readonly IGameServerController _gameServerController;
    private byte[] _gameServerListPacket = Array.Empty<byte>();
    private int _gameServerListPacketSize;

    /// <summary>Creates a new <see cref="GameServerListRequestHandler" />.</summary>
    public GameServerListRequestHandler(IGameServerController gameServerController) {
      _gameServerController = gameServerController;
      _gameServerController.GameServerRegistered += (_, __) => OnGameServerChange();
      _gameServerController.GameServerDeregistered += (_, __) => OnGameServerChange();
      _gameServerController.GameServerUpdated += (_, __) => OnGameServerChange();
    }

    /// <inheritdoc />
    public bool HandlePacket(Client client, Span<byte> packet) {
      if (_gameServerListPacketSize == 0) {
        CreateServerListPacket(_gameServerController.Servers);
      }

      using (var writer = client.Connection.StartWrite(_gameServerListPacketSize)) {
        _gameServerListPacket.AsSpan().Slice(0, _gameServerListPacketSize).CopyTo(writer.Span);
      }

      return true;
    }

    private void CreateServerListPacket(IReadOnlyCollection<GameServer> servers) {
      _gameServerListPacketSize = PacketHelper.GetPacketSize<GameServerList, GameServerList.GameServer>(servers.Count);

      if (_gameServerListPacket.Length < _gameServerListPacketSize) {
        _gameServerListPacket = new byte[_gameServerListPacketSize];
      }

      PacketHelper.CreatePacket<GameServerList, GameServerList.GameServer>(
        servers.Count,
        _gameServerListPacket.AsSpan(),
        out Span<GameServerList.GameServer> serverEntries);

      foreach (var (index, server) in servers.Select((s, i) => (i, s))) {
        serverEntries[index].Code = server.Code;
        serverEntries[index].Load = server.Load;
      }
    }

    private void OnGameServerChange() => _gameServerListPacketSize = 0;
  }
}