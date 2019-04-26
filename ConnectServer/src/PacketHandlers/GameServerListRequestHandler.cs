using System;
using System.Collections.Generic;
using Muwesome.Protocol;

namespace Muwesome.ConnectServer.PacketHandlers {
  internal class GameServerListRequestHandler : IPacketHandler<Client> {
    private readonly IGameServerController _gameServerController;
    private readonly byte[] _gameServerListPacket;
    private bool _gameServerListChanged;

    public GameServerListRequestHandler(IGameServerController gameServerController) {
      _gameServerController = gameServerController;
      _gameServerController.GameServerRegistered += (_, __) => OnGameServerChange();
      _gameServerController.GameServerDeregistered += (_, __) => OnGameServerChange();
      _gameServerController.GameServerUpdated += (_, __) => OnGameServerChange();
      _gameServerListChanged = true;
      // TODO: THREAD SAFETY
    }

    /// <inheritdoc />
    public bool HandlePacket(Client client, Span<byte> packet) {
      throw new NotImplementedException();
    }

    private void OnGameServerChange() => _gameServerListChanged = true;
  }
}