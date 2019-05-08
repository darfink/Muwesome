using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using log4net;
using Muwesome.ConnectServer.Utility;

namespace Muwesome.ConnectServer {
  internal class GameServerController : IGameServerController {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(GameServerController));
    private readonly ConcurrentDictionary<ushort, GameServerEntry> gameServers;

    /// <summary>Initializes a new instance of the <see cref="GameServerController"/> class.</summary>
    public GameServerController() =>
      this.gameServers = new ConcurrentDictionary<ushort, GameServerEntry>();

    /// <inheritdoc />
    public event EventHandler<GameServerEventArgs> GameServerRegistered;

    /// <inheritdoc />
    public event EventHandler<GameServerEventArgs> GameServerUpdated;

    /// <inheritdoc />
    public event EventHandler<GameServerEventArgs> GameServerDeregistered;

    /// <inheritdoc />
    public IReadOnlyCollection<GameServerEntry> Servers => this.gameServers.Values.AsReadOnly();

    /// <inheritdoc />
    public void RegisterServer(GameServerEntry server) {
      if (!this.gameServers.TryAdd(server.Code, server)) {
        throw new ArgumentException($"Conflicting game server code {server.Code}", nameof(server));
      }

      server.PropertyChanged += this.OnGameServerChange;
      this.GameServerRegistered?.Invoke(this, new GameServerEventArgs(server));
      Logger.Info($"Game server registered {server}; current server count {this.gameServers.Count}");
    }

    /// <inheritdoc />
    public void DeregisterServer(GameServerEntry server) {
      if (!this.gameServers.TryRemove(server.Code, out _)) {
        throw new ArgumentException($"Non-existing game server code {server.Code}", nameof(server));
      }

      server.PropertyChanged -= this.OnGameServerChange;
      this.GameServerDeregistered?.Invoke(this, new GameServerEventArgs(server));
      Logger.Info($"Game server deregistered {server}");
    }

    /// <inheritdoc />
    public GameServerEntry GetServerByCode(ushort code) {
      if (this.gameServers.TryGetValue(code, out GameServerEntry server)) {
        return server;
      }

      return null;
    }

    private void OnGameServerChange(object sender, PropertyChangedEventArgs ev) {
      var server = (GameServerEntry)sender;
      Logger.Debug($"Game server update {server}; clients {server.ClientCount}/{server.ClientCapacity}");
      this.GameServerUpdated?.Invoke(this, new GameServerEventArgs(server));
    }
  }
}