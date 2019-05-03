using System;
using System.Linq;
using Muwesome.Interfaces;
using Muwesome.Network;
using Muwesome.ServerCommon;

namespace Muwesome.GameServer {
  /// <summary>A game server.</summary>
  public class GameServer : LifecycleController {
    private readonly IConnectServerRegisterer connectServerRegisterer;

    /// <summary>Initializes a new instance of the <see cref="GameServer"/> class.</summary>
    public GameServer(
        Configuration config,
        IConnectServerRegisterer connectServerRegisterer,
        IClientListener clientListener,
        params ILifecycle[] lifecycleServices)
        : base(lifecycleServices.Prepend(clientListener).ToArray()) {
      this.Config = config;
      this.connectServerRegisterer = connectServerRegisterer;

      // clientListener.AfterClientAccepted += (_, ev) =>
    }

    /// <summary>Gets the server's configuration.</summary>
    public Configuration Config { get; }

    /// <summary>
    /// Gets a value indicating whether the server is registered at the connect server or not.
    /// </summary>
    public bool IsRegistered => this.connectServerRegisterer.IsRegistered;
  }
}