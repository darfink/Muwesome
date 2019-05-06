using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using log4net;
using Muwesome.ConnectServer.Rpc;
using Muwesome.GameServer.Utility;
using Muwesome.Network;

namespace Muwesome.GameServer {
  /// <summary>A connect server registerer.</summary>
  internal class ConnectServerRegisterer : IConnectServerRegisterer, IDisposable {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(ConnectServerRegisterer));
    private readonly IClientTcpListener clientListener;
    private readonly IClientController clientController;
    private readonly Configuration config;
    private CancellationTokenSource cancellationTokenSource;
    private Action updateClientCount;

    /// <summary>Initializes a new instance of the <see cref="ConnectServerRegisterer"/> class.</summary>
    public ConnectServerRegisterer(Configuration config, IClientController clientController, IClientTcpListener clientListener) {
      this.config = config;
      this.clientController = clientController;
      this.clientListener = clientListener;

      clientListener.AfterLifecycleStarted += this.OnClientListenerStarted;
      clientListener.AfterLifecycleEnded += (_, ev) => this.CancelRegistration();

      clientController.ClientSessionStarted += (_, ev) => this.updateClientCount?.Invoke();
      clientController.ClientSessionEnded += (_, ev) => this.updateClientCount?.Invoke();
    }

    /// <inheritdoc />
    public bool IsRegistered { get; private set; }

    /// <inheritdoc />
    public void Dispose() => this.CancelRegistration();

    private async Task RegisterAsync(Channel channel, CancellationToken cancellationToken) {
      Logger.Info($"Registering server at {channel.Target}...");
      using (cancellationToken.Register(() => channel.ShutdownAsync())) {
        await channel.ConnectAsync();
      }

      var client = new GameServerRegistrar.GameServerRegistrarClient(channel);
      using (var registration = client.RegisterGameServer(cancellationToken: cancellationToken)) {
        await registration.RequestStream.WriteAsync(new GameServerParams {
          Register = new GameServerParams.Types.GameServerRegister {
            Code = this.config.ServerCode,
            Host = this.config.ClientListenerEndPoint.ExternalHost ?? this.clientListener.BoundEndPoint.Address.ToString(),
            Port = this.config.ClientListenerEndPoint.ExternalPort ?? (ushort)this.clientListener.BoundEndPoint.Port,
            Status = this.GetServerStatus(),
          },
        });

        this.IsRegistered = true;
        Logger.Info("Game server registered");

        try {
          this.updateClientCount = () => registration.RequestStream.WriteAsync(new GameServerParams { Status = this.GetServerStatus() });
          await registration.ResponseAsync;
        } finally {
          this.updateClientCount = null;
        }
      }

      await channel.ShutdownAsync();
    }

    private void CancelRegistration() {
      var cancelSource = Interlocked.Exchange(ref this.cancellationTokenSource, null);

      if (cancelSource != null) {
        cancelSource.Cancel();
        cancelSource.Dispose();
      }
    }

    private GameServerParams.Types.GameServerStatus GetServerStatus() {
      return checked(new GameServerParams.Types.GameServerStatus {
        ClientCount = (uint)this.clientController.ClientsConnected,
        ClientCapacity = (uint)this.config.MaxConnections,
      });
    }

    private void OnClientListenerStarted(object sender, EventArgs ev) {
      this.cancellationTokenSource = new CancellationTokenSource();
      Channel channel = new Channel(this.config.ConnectServerGrpcHost, this.config.ConnectServerGrpcPort, ChannelCredentials.Insecure);
      this.RegisterAsync(channel, this.cancellationTokenSource.Token)
        .ContinueWith(t => this.OnRegisterComplete(t.Exception));
    }

    private void OnRegisterComplete(Exception ex) {
      if (ex != null) {
        Logger.Error("An unexpected error occured whilst registering game server", ex);
      }

      if (this.IsRegistered) {
        this.IsRegistered = false;
        Logger.Info("Game server deregistered");
      }
    }
  }
}