using System;
using System.Linq;
using Muwesome.GameLogic;
using Muwesome.GameServer.Filters;
using Muwesome.GameServer.Protocol;
using Muwesome.Interfaces;
using Muwesome.Network;
using Muwesome.Protocol.Game;
using Muwesome.ServerCommon;

namespace Muwesome.GameServer {
  /// <summary>A game server.</summary>
  public class GameServer : LifecycleController {
    private readonly IConnectServerRegisterer connectServerRegisterer;
    private readonly IClientSocketFilter[] clientSocketFilters;
    private readonly IClientController clientController;
    private readonly IClientProtocolResolver clientProtocolResolver;
    private readonly GameContext gameContext;

    /// <summary>Initializes a new instance of the <see cref="GameServer"/> class.</summary>
    public GameServer(
        Configuration config,
        IConnectServerRegisterer connectServerRegisterer,
        IClientController clientController,
        IClientListener clientListener,
        IClientProtocolResolver clientProtocolResolver,
        params ILifecycle[] lifecycleServices)
        : base(lifecycleServices.Prepend(clientListener).ToArray()) {
      this.Config = config;
      this.connectServerRegisterer = connectServerRegisterer;
      this.clientController = clientController;
      this.clientProtocolResolver = clientProtocolResolver;
      this.gameContext = new GameContext();

      clientListener.ClientConnected += this.OnClientConnected;

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

    /// <summary>
    /// Gets a value indicating whether the server is registered at the connect server or not.
    /// </summary>
    public bool IsRegistered => this.connectServerRegisterer.IsRegistered;

    /// <inheritdoc />
    public override void Dispose() {
      base.Dispose();
      (this.clientController as IDisposable)?.Dispose();
      (this.connectServerRegisterer as IDisposable)?.Dispose();
    }

    /// <summary>Applies any socket filters.</summary>
    private void OnBeforeClientAccepted(object sender, BeforeClientAcceptEventArgs ev) =>
      ev.RejectClient = this.clientSocketFilters.Any(filter => !filter.OnAllowClientSocketAccept(ev.ClientSocket));

    /// <summary>Configures new clients.</summary>
    private void OnClientConnected(object sender, ClientConnectedEventArgs ev) {
      var endPoint = (sender as IClientTcpListener)?.SourceEndPoint as GameServerEndPoint;
      var clientVersion = endPoint?.ClientVersion ?? this.Config.DefaultClientVersion;
      var clientSerial = endPoint?.ClientSerial ?? this.Config.DefaultClientSerial;

      var protocol = this.clientProtocolResolver.Resolve(clientVersion);
      var client = new Client(ev.ClientConnection, protocol.PacketHandler) {
        Version = clientVersion,
        Serial = clientSerial,
      };

      this.clientController.AddClient(client);
      client.Player.RegisterActions(protocol.PacketDispatcher.Actions);
      this.gameContext.AddPlayer(client.Player);
    }
  }
}