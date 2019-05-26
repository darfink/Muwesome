using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Muwesome.Common;
using Muwesome.GameLogic;
using Muwesome.Network;
using Muwesome.Persistence;

namespace Muwesome.GameServer {
  /// <summary>A game server.</summary>
  public sealed class GameServer : LifecycleController {
    private readonly IClientController clientController;
    private readonly GameContext gameContext;

    /// <summary>Initializes a new instance of the <see cref="GameServer"/> class.</summary>
    internal GameServer(
        Configuration config,
        IClientController clientController,
        GameContext gameContext) {
      this.Config = config;
      this.clientController = clientController;
      this.gameContext = gameContext;
    }

    /// <summary>Gets the server's configuration.</summary>
    public Configuration Config { get; }

    /// <summary>Gets the number of connected clients.</summary>
    public int ClientsConnected => this.clientController.ClientsConnected;

    /// <summary>Adds a client listener to the server.</summary>
    public void AddListener(IClientListener<Client> clientListener) {
      clientListener.ClientConnected += this.OnClientConnected;
      this.AddDependency(clientListener);
    }

    /// <inheritdoc />
    public override void Dispose() {
      base.Dispose();
      (this.clientController as IDisposable)?.Dispose();
    }

    /// <summary>Configures new clients.</summary>
    private void OnClientConnected(object sender, ClientConnectedEventArgs<Client> ev) {
      var client = ev.ConnectedClient;
      var clientDispatch = client.Protocol.PacketDispatcher.CreateDispatch(client);
      client.Player = this.gameContext.AddPlayer(clientDispatch);
      this.clientController.AddClient(client);
    }
  }
}