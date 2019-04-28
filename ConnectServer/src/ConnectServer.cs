using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using Muwesome.Protocol;
using Muwesome.ConnectServer.Plugins;

namespace Muwesome.ConnectServer {
  // TODO: Cmds, blacklist? exit? actvserv? Over gRPC?
  public class ConnectServer : ILifecycle, IDisposable {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(ConnectServer));
    private readonly IGameServerController _gameServerController;
    private readonly IClientController _clientController;
    private readonly IConnectPlugin[] _connectPlugins;
    private readonly ILifecycle[] _lifecycleServices;
    private readonly Stopwatch _startTime;
    private bool _isRunning;

    /// <summary>Creates a new instance of <see cref="ConnectServer" />.</summary>
    public ConnectServer(
        Configuration config,
        IGameServerController gameServerController,
        IClientController clientController,
        IClientListener clientListener,
        IPacketHandler<Client> clientProtocol,
        params ILifecycle[] lifecycleServices
    ) {
      Config = config;
      _gameServerController = gameServerController;
      _clientController = clientController;
      _lifecycleServices = lifecycleServices;
      _lifecycleServices.Append(clientListener);

      clientListener.BeforeClientAccepted += OnBeforeClientAccepted;
      clientListener.AfterClientAccepted += (_, ev) =>
        clientController.AddClient(new Client(ev.ClientConnection, clientProtocol));

      _connectPlugins = new IConnectPlugin[] {
        new CheckMaxConnectionsPlugin(_clientController, config.MaxConnections),
        new CheckMaxConnectionsPerIpPlugin(_clientController, config.MaxConnectionsPerIp),
      };

      _startTime = new Stopwatch();
      _startTime.Start();
    }

    /// <summary>Gets the server's configuration.</summary>
    public Configuration Config { get; }

    /// <summary>Gets the server's uptime.</summary>
    public TimeSpan Uptime => _startTime.Elapsed;

    /// <summary>Gets a list of the server's clients.</summary>
    public IReadOnlyCollection<Client> Clients => _clientController.Clients;

    /// <summary>Gets a list of the server's registered game servers.</summary>
    public IReadOnlyCollection<GameServer> Servers => _gameServerController.Servers;

    /// <inheritdoc />
    public Task Task => Task.WhenAll(_lifecycleServices.Select(service => service.Task));

    /// <inheritdoc />
    public void Start() {
      Logger.Info("Starting ConnectServer...");
      foreach (var service in _lifecycleServices) {
        service.Start();
      }
      _isRunning = true;
      Logger.Info("Server successfully started");
    }

    /// <inheritdoc />
    public void Stop() {
      Logger.Info("Stopping ConnectServer...");
      foreach (var service in _lifecycleServices) {
        service.Stop();
      }
      _isRunning = false;
      Logger.Info("Server stopped");
    }

    /// <inheritdoc />
    public void Dispose() {
      if (_isRunning) {
        Stop();
      }

      (_clientController as IDisposable)?.Dispose();
      foreach (var service in _lifecycleServices.OfType<IDisposable>()) {
        service.Dispose();
      }
    }

    private void OnBeforeClientAccepted(object sender, BeforeClientAcceptEventArgs ev) =>
      ev.RejectClient = _connectPlugins.Any(plugin => !plugin.OnAllowClientSocketAccept(ev.ClientSocket));
  }
}