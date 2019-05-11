using System;
using System.Linq;
using System.Threading.Tasks;
using Muwesome.Common;
using Muwesome.GameLogic;
using Muwesome.GameServer.Protocol;
using Muwesome.Network;
using Muwesome.Network.Tcp;
using Muwesome.Network.Tcp.Filters;
using Muwesome.Protocol.Game;
using Muwesome.ServerCommon;

namespace Muwesome.GameServer {
  /// <summary>A game server.</summary>
  public sealed class GameServer : LifecycleController {
    private readonly IClientController clientController;
    private readonly IClientProtocolResolver clientProtocolResolver;
    private readonly IGameServerRegistrar gameServerRegistrar;
    private readonly GameContext gameContext;
    private GameServerInfo gameServerInfo;
    private bool isRegistered;

    /// <summary>Initializes a new instance of the <see cref="GameServer"/> class.</summary>
    // TODO: Allow multiple client listeners, and remove all TCP logic
    internal GameServer(
        Configuration config,
        IClientController clientController,
        IClientListener clientListener,
        IClientProtocolResolver clientProtocolResolver,
        IGameServerRegistrar gameServerRegistrar)
        : base(clientListener) {
      this.Config = config;
      this.clientController = clientController;
      this.clientProtocolResolver = clientProtocolResolver;
      this.gameServerRegistrar = gameServerRegistrar;
      this.gameContext = new GameContext();

      clientListener.ClientConnected += this.OnClientConnected;
      clientListener.LifecycleStarted += this.OnClientListenerStarted;
      clientListener.LifecycleEnded += this.OnClientListenerStopped;

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
    public override void Dispose() {
      base.Dispose();
      (this.clientController as IDisposable)?.Dispose();
      (this.gameServerRegistrar as IDisposable)?.Dispose();
    }

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

      if (this.gameServerInfo != null) {
        this.gameServerInfo.ClientCount++;
        client.Connection.Disconnected += (_, e) => this.gameServerInfo.ClientCount--;
      }
    }

    private async void OnClientListenerStarted(object sender, LifecycleEventArgs ev) {
      if (sender is IClientTcpListener clientTcpListener) {
        this.gameServerInfo = new GameServerInfo(
          this.Config.ServerCode,
          this.Config.ClientListenerEndPoint.ExternalHost ?? clientTcpListener.BoundEndPoint.Address.ToString(),
          this.Config.ClientListenerEndPoint.ExternalPort ?? (ushort)clientTcpListener.BoundEndPoint.Port,
          (uint)this.ClientsConnected,
          (uint)this.Config.MaxConnections);
        await this.gameServerRegistrar.RegisterGameServerAsync(this.gameServerInfo);
        this.isRegistered = true;
      }
    }

    private async void OnClientListenerStopped(object sender, LifecycleEventArgs ev) {
      if (this.isRegistered) {
        await this.gameServerRegistrar.DeregisterGameServerAsync(this.Config.ServerCode);
      }
    }
  }
}