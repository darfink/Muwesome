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
  }

  public class GameServerEventArgs : EventArgs {
    /// <summary>Constructs a new instance of <see cref="GameServerEventArgs" />.</summary>
    public GameServerEventArgs(GameServer server) => Server = server;

    /// <summary>Gets the game server.</summary>
    public GameServer Server { get; }
  }
}