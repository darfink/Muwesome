using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using Muwesome.Common;
using Muwesome.Network;
using Muwesome.Network.Tcp;
using Muwesome.Network.Tcp.Filters;
using Muwesome.Protocol;

namespace Muwesome.ConnectServer {
  public sealed class ConnectServer : LifecycleController, IGameServerRegistrar {
    private readonly IGameServerController gameServerController;
    private readonly IClientController clientController;

    /// <summary>Initializes a new instance of the <see cref="ConnectServer"/> class.</summary>
    internal ConnectServer(
        Configuration config,
        IGameServerController gameServerController,
        IClientController clientController) {
      this.Config = config;
      this.gameServerController = gameServerController;
      this.clientController = clientController;
    }

    /// <summary>Gets the server's configuration.</summary>
    public Configuration Config { get; }

    /// <summary>Gets the number of connected clients.</summary>
    public int ClientsConnected => this.clientController.ClientsConnected;

    /// <inheritdoc />
    public int GameServersRegistered => this.gameServerController.GameServersRegistered;

    /// <summary>Adds a client listener to the server.</summary>
    public void AddListener(IClientListener<Client> clientListener) {
      clientListener.ClientConnected += (_, ev) => this.clientController.AddClient(ev.ConnectedClient);
      this.AddDependency(clientListener);
    }

    /// <inheritdoc />
    public Task RegisterGameServerAsync(GameServerInfo server) {
      this.gameServerController.RegisterGameServer(server);
      return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task DeregisterGameServerAsync(ushort serverCode) {
      this.gameServerController.DeregisterGameServer(serverCode);
      return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override void Dispose() {
      base.Dispose();
      (this.clientController as IDisposable)?.Dispose();
    }
  }
}