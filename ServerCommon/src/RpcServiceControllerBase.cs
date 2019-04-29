using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using log4net;

namespace Muwesome.ServerCommon {
  public abstract class RpcServiceControllerBase : ILifecycle, IDisposable {
    private readonly ServerPort[] serverPorts;
    private CancellationTokenSource cancellationSource;
    private Server grpcServer;

    /// <summary>Initializes a new instance of the <see cref="RpcServiceControllerBase"/> class.</summary>
    public RpcServiceControllerBase(params ServerPort[] serverPorts) {
      this.serverPorts = serverPorts;
    }

    /// <inheritdoc />
    public Task Task => this.grpcServer?.ShutdownTask ?? Task.CompletedTask;

    /// <summary>Gets the logger instance.</summary>
    protected ILog Logger => LogManager.GetLogger(this.GetType());

    /// <inheritdoc />
    public void Start() {
      if (this.grpcServer != null) {
        throw new InvalidOperationException("The RPC service controller is already running");
      }

      this.cancellationSource = new CancellationTokenSource();
      this.grpcServer = new Server();

      foreach (var service in this.OnRegisterServices(this.cancellationSource.Token)) {
        this.grpcServer.Services.Add(service);
      }

      foreach (var serverPort in this.serverPorts) {
        this.grpcServer.Ports.Add(serverPort);
      }

      this.grpcServer.Start();
      var endPoints = this.grpcServer.Ports
        .Select(server => $"{server.Host}:{server.BoundPort}");
      this.Logger.Info($"RPC service started; listening on {string.Join(", ", endPoints)}");
    }

    /// <inheritdoc />
    public void Stop() {
      this.cancellationSource.Cancel();
      this.cancellationSource.Dispose();
      this.grpcServer.ShutdownAsync().Wait();
      this.grpcServer = null;
      this.Logger.Info("RPC service stopped");
    }

    /// <inheritdoc />
    public void Dispose() {
      if (!this.Task.IsCompleted) {
        this.Stop();
      }
    }

    /// <summary>Called when the RPC services are registered.</summary>
    protected abstract IEnumerable<ServerServiceDefinition> OnRegisterServices(CancellationToken token);
  }
}