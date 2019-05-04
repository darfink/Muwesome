using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using Muwesome.ConnectServer.Filters;
using Muwesome.Interfaces;
using Muwesome.Network;
using Muwesome.Protocol;
using Muwesome.ServerCommon;

namespace Muwesome.ConnectServer {
  // TODO: Cmds, blacklist? exit? actvserv? Over gRPC?
  public class ConnectServer : LifecycleController {
    private readonly IGameServerController gameServerController;
    private readonly IClientSocketFilter[] clientSocketFilters;
    private readonly IClientController clientController;

    /// <summary>Initializes a new instance of the <see cref="ConnectServer"/> class.</summary>
    public ConnectServer(
        Configuration config,
        IGameServerController gameServerController,
        IClientController clientController,
        IClientListener clientListener,
        IPacketHandler<Client> clientProtocol,
        params ILifecycle[] lifecycleServices)
        : base(lifecycleServices.Prepend(clientListener).ToArray()) {
      this.Config = config;
      this.gameServerController = gameServerController;
      this.clientController = clientController;

      clientListener.ClientConnected += (_, ev) =>
        clientController.AddClient(new Client(ev.ClientConnection, clientProtocol));

      if (clientListener is IClientTcpListener clientTcpListener) {
        clientTcpListener.BeforeClientAccepted += this.OnBeforeClientAccepted;
        this.clientSocketFilters = new IClientSocketFilter[] {
          new MaxConnectionsFilter(this.clientController, config.MaxConnections),
          new MaxConnectionsPerIpFilter(this.clientController, config.MaxConnectionsPerIp),
        };
      }
    }

    /// <summary>Gets the server's configuration.</summary>
    public Configuration Config { get; }

    /// <summary>Gets the number of connected clients.</summary>
    public int ClientsConnected => this.clientController.ClientsConnected;

    /// <summary>Gets a list of the server's registered game servers.</summary>
    public IReadOnlyCollection<GameServerEntry> Servers => this.gameServerController.Servers;

    /// <inheritdoc />
    public override void Dispose() {
      base.Dispose();
      (this.clientController as IDisposable)?.Dispose();
    }

    private void OnBeforeClientAccepted(object sender, BeforeClientAcceptEventArgs ev) =>
      ev.RejectClient = this.clientSocketFilters.Any(filter => !filter.OnAllowClientSocketAccept(ev.ClientSocket));
  }
}