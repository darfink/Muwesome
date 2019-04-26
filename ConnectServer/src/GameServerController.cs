using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using log4net;
using Muwesome.ConnectServer.Utility;

namespace Muwesome.ConnectServer {
  internal class GameServerController : IGameServerController {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(GameServerController));
    private ConcurrentDictionary<byte, GameServer> _gameServers;

    /// <summary>Creates a new <see cref="GameServerController" />.</summary>
    public GameServerController() =>
      _gameServers = new ConcurrentDictionary<byte, GameServer>();

    /// <inheritdoc />
    public event EventHandler<GameServerEventArgs> GameServerRegistered;

    /// <inheritdoc />
    public event EventHandler<GameServerEventArgs> GameServerUpdated;

    /// <inheritdoc />
    public event EventHandler<GameServerEventArgs> GameServerDeregistered;

    /// <inheritdoc />
    public IReadOnlyCollection<GameServer> Servers => _gameServers.Values.AsReadOnly();

    /// <inheritdoc />
    public void RegisterServer(GameServer server) {
      if (!_gameServers.TryAdd(server.Id, server)) {
        throw new ArgumentException($"Conflicting game server ID {server.Id}", nameof(server));
      }

      server.PropertyChanged += OnGameServerChange;
      GameServerRegistered?.Invoke(this, new GameServerEventArgs(server));
      Logger.Info($"Game server registered {server}; current server count {_gameServers.Count}");
    }

    /// <inheritdoc />
    public void DeregisterServer(GameServer server) {
      if (!_gameServers.TryRemove(server.Id, out _)) {
        throw new ArgumentException($"Non-existing game server ID {server.Id}", nameof(server));
      }

      server.PropertyChanged -= OnGameServerChange;
      GameServerDeregistered?.Invoke(this, new GameServerEventArgs(server));
      Logger.Info($"Game server deregistered {server}");
    }

    private void OnGameServerChange(object sender, PropertyChangedEventArgs ev) {
      var server = (GameServer)sender;
      Logger.Debug($"Game server updated {server}; {server.ClientCount}/{server.ClientCapacity}");
      GameServerUpdated?.Invoke(this, new GameServerEventArgs(server));
    }
  }
}