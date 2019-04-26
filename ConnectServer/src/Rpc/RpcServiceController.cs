using System;
using System.Threading.Tasks;
using System.Linq;
using log4net;
using Grpc.Core;

namespace Muwesome.ConnectServer.Rpc {
  public class RpcServiceController : ILifecycle, IDisposable {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(RpcServiceController));
    private readonly Server _grpcServer;

    public RpcServiceController(Configuration config, IGameServerController gameServerController) {
      _grpcServer = new Server() {
        Services = { GameServerRegister.BindService(new GameServerRegisterService(gameServerController)) },
        Ports = { new ServerPort(config.GrpcListenerHost, config.GrpcListenerPort, ServerCredentials.Insecure) },
      };
    }

    /// <inheritdoc />
    public Task Task => _grpcServer.ShutdownTask ?? Task.CompletedTask;

    /// <inheritdoc />
    public void Start() {
      _grpcServer.Start();
      var endPoints = _grpcServer.Ports
        .Select(server => $"{server.Host}:{server.BoundPort}");
      Logger.Info($"RPC service started; listening on {String.Join(", ", endPoints)}");
    }

    /// <inheritdoc />
    public void Stop() {
      _grpcServer.ShutdownAsync().Wait();
      Logger.Info("RPC service stopped");
    }

    /// <inheritdoc />
    public void Dispose() => Stop();
  }
}