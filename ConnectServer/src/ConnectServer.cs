using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using Muwesome.ConnectServer.Plugins;
using Muwesome.Protocol;

namespace Muwesome.ConnectServer {
  // TODO: Cmds, blacklist? exit? actvserv? Over gRPC?
  public class ConnectServer : ILifecycle, IDisposable {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(ConnectServer));
    private readonly IGameServerController gameServerController;
    private readonly IClientController clientController;
    private readonly IConnectPlugin[] connectPlugins;
    private readonly ILifecycle[] lifecycleServices;
    private readonly Stopwatch startTime;
    private bool isRunning;

    /// <summary>Initializes a new instance of the <see cref="ConnectServer"/> class.</summary>
    public ConnectServer(
        Configuration config,
        IGameServerController gameServerController,
        IClientController clientController,
        IClientListener clientListener,
        IPacketHandler<Client> clientProtocol,
        params ILifecycle[] lifecycleServices) {
      this.Config = config;
      this.gameServerController = gameServerController;
      this.clientController = clientController;
      this.lifecycleServices = lifecycleServices.Prepend(clientListener).ToArray();

      clientListener.BeforeClientAccepted += this.OnBeforeClientAccepted;
      clientListener.AfterClientAccepted += (_, ev) =>
        clientController.AddClient(new Client(ev.ClientConnection, clientProtocol));

      this.connectPlugins = new IConnectPlugin[] {
        new CheckMaxConnectionsPlugin(this.clientController, config.MaxConnections),
        new CheckMaxConnectionsPerIpPlugin(this.clientController, config.MaxConnectionsPerIp),
      };

      this.startTime = new Stopwatch();
      this.startTime.Start();
    }

    /// <summary>Gets the server's configuration.</summary>
    public Configuration Config { get; }

    /// <summary>Gets the server's uptime.</summary>
    public TimeSpan Uptime => this.startTime.Elapsed;

    /// <summary>Gets a list of the server's clients.</summary>
    public IReadOnlyCollection<Client> Clients => this.clientController.Clients;

    /// <summary>Gets a list of the server's registered game servers.</summary>
    public IReadOnlyCollection<GameServer> Servers => this.gameServerController.Servers;

    /// <inheritdoc />
    public Task Task => Task.WhenAll(this.lifecycleServices.Select(service => service.Task));

    /// <inheritdoc />
    public void Start() {
      Logger.Info("Starting ConnectServer...");
      foreach (var service in this.lifecycleServices) {
        service.Start();
      }

      this.isRunning = true;
      Logger.Info("Server successfully started");
    }

    /// <inheritdoc />
    public void Stop() {
      Logger.Info("Stopping ConnectServer...");
      foreach (var service in this.lifecycleServices) {
        service.Stop();
      }

      this.isRunning = false;
      Logger.Info("Server stopped");
    }

    /// <inheritdoc />
    public void Dispose() {
      if (this.isRunning) {
        this.Stop();
      }

      (this.clientController as IDisposable)?.Dispose();
      foreach (var service in this.lifecycleServices.OfType<IDisposable>()) {
        service.Dispose();
      }
    }

    private void OnBeforeClientAccepted(object sender, BeforeClientAcceptEventArgs ev) =>
      ev.RejectClient = this.connectPlugins.Any(plugin => !plugin.OnAllowClientSocketAccept(ev.ClientSocket));
  }
}