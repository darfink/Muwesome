using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using log4net;

namespace Muwesome.ConnectServer.Rpc {
  public class RpcServiceController : ILifecycle, IDisposable {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(RpcServiceController));
    private readonly IGameServerController gameServerController;
    private readonly Configuration config;
    private CancellationTokenSource cancellationSource;
    private Server grpcServer;

    /// <summary>Initializes a new instance of the <see cref="RpcServiceController"/> class.</summary>
    public RpcServiceController(Configuration config, IGameServerController gameServerController) {
      this.gameServerController = gameServerController;
      this.config = config;
    }

    /// <inheritdoc />
    public Task Task => this.grpcServer?.ShutdownTask ?? Task.CompletedTask;

    /// <inheritdoc />
    public void Start() {
      if (this.grpcServer != null) {
        throw new InvalidOperationException("The RPC service controller is already running");
      }

      this.cancellationSource = new CancellationTokenSource();
      this.grpcServer = new Server() {
        Services = { GameServerRegister.BindService(new GameServerRegisterService(this.gameServerController, this.cancellationSource.Token)) },
        Ports = { new ServerPort(this.config.GrpcListenerHost, this.config.GrpcListenerPort, ServerCredentials.Insecure) },
      };

      this.grpcServer.Start();
      var endPoints = this.grpcServer.Ports
        .Select(server => $"{server.Host}:{server.BoundPort}");
      Logger.Info($"RPC service started; listening on {string.Join(", ", endPoints)}");
    }

    /// <inheritdoc />
    public void Stop() {
      this.cancellationSource.Cancel();
      this.cancellationSource.Dispose();
      this.grpcServer.ShutdownAsync().Wait();
      this.grpcServer = null;
      Logger.Info("RPC service stopped");
    }

    /// <inheritdoc />
    public void Dispose() {
      if (!this.Task.IsCompleted) {
        this.Stop();
      }
    }
  }
}