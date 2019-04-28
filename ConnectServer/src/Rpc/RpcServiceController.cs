using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using log4net;
using Grpc.Core;

namespace Muwesome.ConnectServer.Rpc {
  public class RpcServiceController : ILifecycle, IDisposable {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(RpcServiceController));
    private readonly IGameServerController _gameServerController;
    private readonly Configuration _config;
    private CancellationTokenSource _cancellationSource;
    private Server _grpcServer;

    /// <summary>Creates a new <see cref="RpcServiceController" />.</summary>
    public RpcServiceController(Configuration config, IGameServerController gameServerController) {
      _gameServerController = gameServerController;
      _config = config;
    }

    /// <inheritdoc />
    public Task Task => _grpcServer?.ShutdownTask ?? Task.CompletedTask;

    /// <inheritdoc />
    public void Start() {
      if (_grpcServer != null) {
        throw new InvalidOperationException("The RPC service controller is already running");
      }

      _cancellationSource = new CancellationTokenSource();
      _grpcServer = new Server() {
        Services = { GameServerRegister.BindService(new GameServerRegisterService(_gameServerController, _cancellationSource.Token)) },
        Ports = { new ServerPort(_config.GrpcListenerHost, _config.GrpcListenerPort, ServerCredentials.Insecure) },
      };

      _grpcServer.Start();
      var endPoints = _grpcServer.Ports
        .Select(server => $"{server.Host}:{server.BoundPort}");
      Logger.Info($"RPC service started; listening on {String.Join(", ", endPoints)}");
    }

    /// <inheritdoc />
    public void Stop() {
      _cancellationSource.Cancel();
      _cancellationSource.Dispose();
      _grpcServer.ShutdownAsync().Wait();
      _grpcServer = null;
      Logger.Info("RPC service stopped");
    }

    /// <inheritdoc />
    public void Dispose() {
      if (!Task.IsCompleted) {
        Stop();
      }
    }
  }
}