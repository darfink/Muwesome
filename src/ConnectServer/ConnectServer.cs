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
  // TODO: Cmds, blacklist? exit? actvserv? Over gRPC?
  public sealed class ConnectServer : LifecycleController, IGameServerRegistrar {
    private readonly IGameServerController gameServerController;
    private readonly IClientController clientController;

    /// <summary>Initializes a new instance of the <see cref="ConnectServer"/> class.</summary>
    internal ConnectServer(
        Configuration config,
        IGameServerController gameServerController,
        IClientController clientController,
        IClientListener clientListener,
        IPacketHandler<Client> clientProtocol)
        : base(clientListener) {
      this.Config = config;
      this.gameServerController = gameServerController;
      this.clientController = clientController;

      clientListener.ClientConnected += (_, ev) =>
        clientController.AddClient(new Client(ev.ClientConnection, clientProtocol));

      if (clientListener is IClientTcpListener clientTcpListener) {
        new MaxConnectionsFilter(clientTcpListener, config.MaxConnections);
        new MaxConnectionsPerIpFilter(clientTcpListener, config.MaxConnectionsPerIp);
      }
    }

    /// <summary>Gets the server's configuration.</summary>
    public Configuration Config { get; }

    /// <summary>Gets the number of connected clients.</summary>
    public int ClientsConnected => this.clientController.ClientsConnected;

    /// <inheritdoc />
    public int GameServersRegistered => this.gameServerController.GameServersRegistered;

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