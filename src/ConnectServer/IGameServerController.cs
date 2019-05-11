using System;
using System.Collections.Generic;
using Muwesome.Interfaces;

namespace Muwesome.ConnectServer {
  internal interface IGameServerController : IGameServerRegistrar {
    /// <summary>An event that is raised when a game server is registered.</summary>
    event EventHandler<GameServerEventArgs> GameServerRegistered;

    /// <summary>An event that is raised when a game server is updated.</summary>
    event EventHandler<GameServerEventArgs> GameServerUpdated;

    /// <summary>An event that is raised when a game server is deregistered.</summary>
    event EventHandler<GameServerEventArgs> GameServerDeregistered;

    /// <summary>Gets a list of registered servers.</summary>
    IReadOnlyCollection<GameServerInfo> GameServers { get; }

    /// <summary>Gets a registered game server by its code.</summary>
    GameServerInfo GetGameServerByCode(ushort code);
  }

  internal class GameServerEventArgs : EventArgs {
    /// <summary>Initializes a new instance of the <see cref="GameServerEventArgs"/> class.</summary>
    public GameServerEventArgs(GameServerInfo server) => this.Server = server;

    /// <summary>Gets the game server.</summary>
    public GameServerInfo Server { get; }
  }
}