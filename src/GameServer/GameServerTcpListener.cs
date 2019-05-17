using System;
using System.Threading.Tasks;
using Muwesome.Common;
using Muwesome.GameServer.Protocol;
using Muwesome.Network;
using Muwesome.Network.Tcp;
using Muwesome.Network.Tcp.Filters;
using Muwesome.Packet.IO.SimpleModulus;
using Muwesome.Packet.IO.Xor;

namespace Muwesome.GameServer {
  /// <summary>A game server TCP listener from incoming clients.</summary>
  internal class GameServerTcpListener : ClientTcpListenerBase<Client> {
    private readonly IClientProtocolResolver clientProtocolResolver;
    private readonly IClientController clientController;
    private readonly IGameServerRegistrar gameServerRegistrar;
    private readonly Configuration config;
    private GameServerInfo gameServerInfo;

    /// <summary>Initializes a new instance of the <see cref="GameServerTcpListener" /> class.</summary>
    // TODO: The determined end point should be injected
    public GameServerTcpListener(
        Configuration config,
        IClientController clientController,
        IClientProtocolResolver clientProtocolResolver,
        IGameServerRegistrar gameServerRegistrar)
        : base(config.ClientListenerEndPoint, config.MaxPacketSize) {
      this.config = config;
      this.gameServerRegistrar = gameServerRegistrar;
      this.clientProtocolResolver = clientProtocolResolver;
      this.clientController = clientController;
      this.clientController.ClientSessionStarted += this.OnServerClientsChanged;
      this.clientController.ClientSessionEnded += this.OnServerClientsChanged;
      this.ConfigureEncryption();
    }

    /// <summary>Gets the listener's <see cref="GameServerEndPoint" />.</summary>
    private GameServerEndPoint GameServerEndPoint => this.SourceEndPoint as GameServerEndPoint;

    /// <inheritdoc />
    protected override void OnListenerStarted() {
      this.gameServerInfo = new GameServerInfo(
        this.config.ServerCode,
        this.GameServerEndPoint.ExternalHost ?? this.BoundEndPoint.Address.ToString(),
        this.GameServerEndPoint.ExternalPort ?? (ushort)this.BoundEndPoint.Port,
        (uint)this.clientController.ClientsConnected,
        (uint)this.config.MaxConnections);
      this.gameServerRegistrar.RegisterGameServerAsync(this.gameServerInfo)
        .ContinueWith(t => this.OnServerRegisterException(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
      base.OnListenerStarted();
    }

    /// <inheritdoc />
    protected override void OnListenerStopped() {
      this.gameServerRegistrar.DeregisterGameServerAsync(this.config.ServerCode)
        .ContinueWith(t => this.OnServerRegisterException(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
      base.OnListenerStopped();
    }

    /// <inheritdoc />
    protected override Client OnConnectionEstablished(IConnection connection) {
      var clientVersion = this.GameServerEndPoint.ClientVersion ?? this.config.DefaultClientVersion;
      var clientSerial = this.GameServerEndPoint.ClientSerial ?? this.config.DefaultClientSerial;
      var clientProtocol = this.clientProtocolResolver.Resolve(clientVersion);
      return new Client(connection, clientProtocol) {
        MaxIdleTime = this.config.MaxIdleTime,
        Version = clientVersion,
        Serial = clientSerial,
      };
    }

    private void OnServerRegisterException(Exception ex) =>
      this.Logger.Error("An error occurred whilst registering server", ex);

    private void OnServerClientsChanged(object sender, ClientSessionEventArgs ev) {
      if (this.gameServerInfo != null) {
        this.gameServerInfo.ClientCount = (uint)this.clientController.ClientsConnected;
      }
    }

    private void ConfigureEncryption() {
      this.Decryption = reader => new XorPipelineDecryptor(new SimpleModulusPipelineDecryptor(reader).Reader);
      this.Encryption = null;
    }
  }
}