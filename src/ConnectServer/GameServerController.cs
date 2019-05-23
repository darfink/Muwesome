using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using log4net;
using Muwesome.Common;
using Muwesome.ConnectServer.Utility;

namespace Muwesome.ConnectServer {
  internal class GameServerController : IGameServerController {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(GameServerController));
    private readonly ConcurrentDictionary<ushort, GameServerInfo> gameServers = new ConcurrentDictionary<ushort, GameServerInfo>();

    /// <inheritdoc />
    public event EventHandler<GameServerEventArgs> GameServerRegistered;

    /// <inheritdoc />
    public event EventHandler<GameServerEventArgs> GameServerUpdated;

    /// <inheritdoc />
    public event EventHandler<GameServerEventArgs> GameServerDeregistered;

    /// <inheritdoc />
    public int GameServersRegistered => this.gameServers.Count;

    /// <inheritdoc />
    public IReadOnlyCollection<GameServerInfo> GameServers => this.gameServers.Values.AsReadOnly();

    /// <inheritdoc />
    public void RegisterGameServer(GameServerInfo server) {
      if (!this.gameServers.TryAdd(server.Code, server)) {
        throw new ArgumentException($"Conflicting game server code {server.Code}", nameof(server));
      }

      server.PropertyChanged += this.OnGameServerChange;
      this.GameServerRegistered?.Invoke(this, new GameServerEventArgs(server));
      Logger.InfoFormat("Game server registered; {0} (server count: {1})", server, this.gameServers.Count);
    }

    /// <inheritdoc />
    public void DeregisterGameServer(ushort serverCode) {
      if (!this.gameServers.TryRemove(serverCode, out GameServerInfo server)) {
        throw new ArgumentException($"Non-existing game server code {serverCode}", nameof(serverCode));
      }

      server.PropertyChanged -= this.OnGameServerChange;
      this.GameServerDeregistered?.Invoke(this, new GameServerEventArgs(server));
      Logger.InfoFormat("Game server deregistered {0}", server);
    }

    /// <inheritdoc />
    public GameServerInfo GetGameServerByCode(ushort serverCode) {
      if (this.gameServers.TryGetValue(serverCode, out GameServerInfo server)) {
        return server;
      }

      return null;
    }

    private void OnGameServerChange(object sender, PropertyChangedEventArgs ev) {
      var server = (GameServerInfo)sender;
      Logger.DebugFormat("Game server update {0}; clients {1}/{2}", server, server.ClientCount, server.ClientCapacity);
      this.GameServerUpdated?.Invoke(this, new GameServerEventArgs(server));
    }
  }
}