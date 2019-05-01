using System;
using Muwesome.Interfaces;
using Muwesome.ServerCommon;

namespace Muwesome.GameServer {
  /// <summary>A game server.</summary>
  public class GameServer : LifecycleController {
    /// <summary>Initializes a new instance of the <see cref="GameServer"/> class.</summary>
    public GameServer(Configuration config, params ILifecycle[] lifecycleServices)
        : base(lifecycleServices) {
      this.Config = config;
    }

    /// <summary>Gets the server's configuration.</summary>
    public Configuration Config { get; }
  }
}