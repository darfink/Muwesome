using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using log4net;
using Muwesome.ConnectServer.Rpc;
using Muwesome.Network;

namespace Muwesome.GameServer {
  /// <summary>A connect server registerer.</summary>
  internal class ConnectServerRegisterer : IConnectServerRegisterer, IDisposable {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(ClientTcpListener));
    private readonly Configuration config;
    private CancellationTokenSource cancellationTokenSource;

    /// <summary>Initializes a new instance of the <see cref="ConnectServerRegisterer"/> class.</summary>
    public ConnectServerRegisterer(Configuration config, IClientListener clientListener) {
      this.config = config;
      clientListener.AfterLifecycleStarted += this.OnClientListenerStarted;
      clientListener.AfterLifecycleEnded += (_, ev) => this.CancelRegistration();
    }

    /// <inheritdoc />
    public bool IsRegistered { get; private set; }

    /// <inheritdoc />
    public void Dispose() => this.CancelRegistration();

    private void OnClientListenerStarted(object sender, EventArgs ev) {
      this.cancellationTokenSource = new CancellationTokenSource();
      Channel channel = new Channel("127.0.0.1:22336", ChannelCredentials.Insecure);
      this.RegisterAsync(channel, this.cancellationTokenSource.Token)
        .ContinueWith(t => this.OnRegisterComplete(t.Exception));
    }

    private void OnRegisterComplete(Exception ex) {
      if (ex != null) {
        Logger.Error("An unexpected error occured whilst registering game server", ex);
      }

      this.IsRegistered = false;
      Logger.Info("Game server deregistered");
    }

    private async Task RegisterAsync(Channel channel, CancellationToken cancellationToken) {
      while (channel.State != ChannelState.Ready && !cancellationToken.IsCancellationRequested) {
        // TODO: Exponential backoff?
        await channel.ConnectAsync(DateTime.UtcNow.AddSeconds(3));
      }

      if (cancellationToken.IsCancellationRequested) {
        await Task.FromCanceled(cancellationToken);
      }

      var client = new GameServerRegistrar.GameServerRegistrarClient(channel);

      using (var registration = client.RegisterGameServer(cancellationToken: cancellationToken)) {
        await registration.RequestStream.WriteAsync(new GameServerParams {
          Register = {
            Code = 0,
            Host = "majs",
            Port = 3306,
            Status = {
              ClientCount = 0,
              ClientCapacity = checked((uint)this.config.MaxConnections),
            },
          },
        });
        this.IsRegistered = true;
        Logger.Info("Game server registered");

        await registration.ResponseAsync;
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
  }
}