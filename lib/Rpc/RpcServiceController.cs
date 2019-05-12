using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using log4net;
using Muwesome.Common;

namespace Muwesome.Rpc {
  /// <summary>A factory for RPC services.</summary>
  public delegate ServerServiceDefinition ServiceFactory(CancellationToken cancellationToken);

  /// <summary>A controller for RPC services.</summary>
  public class RpcServiceController : ILifecycle, IDisposable {
    private readonly ServerPort[] serverPorts;
    private readonly List<ServiceFactory> registeredServices;
    private CancellationTokenSource cancellationSource;
    private Server grpcServer;

    /// <summary>Initializes a new instance of the <see cref="RpcServiceController"/> class.</summary>
    public RpcServiceController(params ServerPort[] serverPorts) {
      this.registeredServices = new List<ServiceFactory>();
      this.serverPorts = serverPorts;
    }

    /// <inheritdoc />
    public event EventHandler<LifecycleEventArgs> LifecycleStarted;

    /// <inheritdoc />
    public event EventHandler<LifecycleEventArgs> LifecycleEnded;

    /// <inheritdoc />
    public Task ShutdownTask => this.grpcServer?.ShutdownTask ?? Task.CompletedTask;

    /// <summary>Gets the logger instance.</summary>
    protected ILog Logger => LogManager.GetLogger(this.GetType());

    /// <summary>Registers a new service to the controller.</summary>
    public void RegisterService(ServiceFactory serviceRegistrar) =>
      this.registeredServices.Add(serviceRegistrar);

    /// <inheritdoc />
    public void Start() {
      if (this.grpcServer != null) {
        throw new InvalidOperationException("The RPC service controller is already running");
      }

      this.cancellationSource = new CancellationTokenSource();
      this.grpcServer = new Server();

      foreach (var serviceFactory in this.registeredServices) {
        this.grpcServer.Services.Add(serviceFactory(this.cancellationSource.Token));
      }

      foreach (var serverPort in this.serverPorts) {
        this.grpcServer.Ports.Add(serverPort);
      }

      this.grpcServer.Start();
      var endPoints = this.grpcServer.Ports
        .Select(server => $"{server.Host}:{server.BoundPort}");
      this.LifecycleStarted?.Invoke(this, new LifecycleEventArgs());
      this.Logger.Info($"RPC service started; listening on {string.Join(", ", endPoints)}");
    }

    /// <inheritdoc />
    public void Stop() {
      var grpcServer = Interlocked.Exchange(ref this.grpcServer, null);
      if (grpcServer == null) {
        return;
      }

      using (this.cancellationSource) {
        this.cancellationSource.Cancel();
      }

      grpcServer.ShutdownAsync().Wait();
      grpcServer = null;

      this.LifecycleEnded?.Invoke(this, new LifecycleEventArgs());
      this.Logger.Info("RPC service stopped");
    }

    /// <inheritdoc />
    public void Dispose() {
      if (this.grpcServer != null) {
        this.Stop();
      }
    }
  }
}