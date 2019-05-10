using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using log4net;
using Muwesome.GameServer.Utility;
using Muwesome.Network.Tcp;
using Muwesome.Rpc.ConnectServer;

namespace Muwesome.GameServer {
  /// <summary>A connect server registerer.</summary>
  internal class GameServerRegisterer : IGameServerRegisterer, IDisposable {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(GameServerRegisterer));
    private readonly IClientTcpListener clientListener;
    private readonly IClientController clientController;
    private readonly Configuration config;
    private CancellationTokenSource cancellationTokenSource;
    private Action updateClientCount;

    /// <summary>Initializes a new instance of the <see cref="GameServerRegisterer"/> class.</summary>
    public GameServerRegisterer(Configuration config, IClientController clientController, IClientTcpListener clientListener) {
      this.config = config;
      this.clientController = clientController;
      this.clientListener = clientListener;

      clientListener.LifecycleStarted += this.OnClientListenerStarted;
      clientListener.LifecycleEnded += (_, ev) => this.CancelRegistration();

      clientController.ClientSessionStarted += (_, ev) => this.updateClientCount?.Invoke();
      clientController.ClientSessionEnded += (_, ev) => this.updateClientCount?.Invoke();
    }

    /// <inheritdoc />
    public Task ShutdownTask { get; private set; } = Task.CompletedTask;

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
        await registration.RequestStream.WriteAsync(new GameServerRequest {
          Register = new GameServerRequest.Types.Register {
            Code = this.config.ServerCode,
            Host = this.config.ClientListenerEndPoint.ExternalHost ?? this.clientListener.BoundEndPoint.Address.ToString(),
            Port = this.config.ClientListenerEndPoint.ExternalPort ?? (ushort)this.clientListener.BoundEndPoint.Port,
            Status = this.GetServerStatus(),
          },
        });

        this.IsRegistered = true;
        Logger.Info("Server registered");

        try {
          this.updateClientCount = () => registration.RequestStream.WriteAsync(new GameServerRequest { Status = this.GetServerStatus() });
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

    private GameServerRequest.Types.StatusUpdate GetServerStatus() {
      return checked(new GameServerRequest.Types.StatusUpdate {
        ClientCount = (uint)this.clientController.ClientsConnected,
        ClientCapacity = (uint)this.config.MaxConnections,
      });
    }

    private void OnClientListenerStarted(object sender, EventArgs ev) {
      this.cancellationTokenSource = new CancellationTokenSource();
      Channel channel = new Channel(this.config.ConnectServerGrpcHost, this.config.ConnectServerGrpcPort, ChannelCredentials.Insecure);
      this.ShutdownTask = this.RegisterAsync(channel, this.cancellationTokenSource.Token)
        .ContinueWith(t => this.OnRegisterComplete(t.Exception));
    }

    private void OnRegisterComplete(Exception ex) {
      if (ex != null && ex.GetExceptionByType<RpcException>()?.StatusCode != StatusCode.Cancelled) {
        Logger.Error("An unexpected error occurred during game server registration", ex);
      }

      if (this.IsRegistered) {
        this.IsRegistered = false;
        Logger.Info("Server deregistered");
      }
    }
  }
}