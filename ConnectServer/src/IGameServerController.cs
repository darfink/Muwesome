using System;
using System.Collections.Generic;

namespace Muwesome.ConnectServer {
  public interface IGameServerController {
    /// <summary>An event that is raised when a game server is registered.</summary>
    event EventHandler<GameServerEventArgs> GameServerRegistered;

    /// <summary>An event that is raised when a game server is updated.</summary>
    event EventHandler<GameServerEventArgs> GameServerUpdated;

    /// <summary>An event that is raised when a game server is deregistered.</summary>
    event EventHandler<GameServerEventArgs> GameServerDeregistered;

    /// <summary>Gets a list of registered servers.</summary>
    IReadOnlyCollection<GameServer> Servers { get; }

    /// <summary>Registers a new game server</summary>
    void RegisterServer(GameServer server);

    /// <summary>Deregisters an existing game server</summary>
    void DeregisterServer(GameServer server);

    /// <summary>Gets a registered game server by its code.</summary>
    GameServer GetServerByCode(ushort code);
  }

  public class GameServerEventArgs : EventArgs {
    /// <summary>Initializes a new instance of the <see cref="GameServerEventArgs"/> class.</summary>
    public GameServerEventArgs(GameServer server) => this.Server = server;

    /// <summary>Gets the game server.</summary>
    public GameServer Server { get; }
  }
}