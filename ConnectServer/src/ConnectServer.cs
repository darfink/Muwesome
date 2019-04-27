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
    private readonly IClientListener _clientListener;
    private readonly IConnectPlugin[] _connectPlugins;
    private readonly ILifecycle[] _metaServices;
    private readonly Stopwatch _startTime;
    private bool _isRunning;

    public ConnectServer(
        Configuration config,
        IGameServerController gameServerController,
        IClientController clientController,
        IClientListener clientListener,
        IPacketHandler<Client> clientProtocol,
        params ILifecycle[] metaServices
    ) {
      Config = config;
      _metaServices = metaServices;
      _gameServerController = gameServerController;
      _clientController = clientController;

      _clientListener = clientListener;
      _clientListener.BeforeClientAccepted += OnBeforeClientAccepted;
      _clientListener.AfterClientAccepted += (_, ev) =>
        clientController.AddClient(new Client(ev.ClientConnection, clientProtocol));

      _connectPlugins = new IConnectPlugin[] {
        new CheckMaxConnectionsPlugin(_clientController, config.MaxConnections),
        new CheckMaxConnectionsPerIpPlugin(_clientController, config.MaxConnectionsPerIp),
      };

      _startTime = new Stopwatch();
      _startTime.Start();
    }

    public Configuration Config { get; }

    public TimeSpan Uptime => _startTime.Elapsed;

    public IReadOnlyCollection<Client> Clients => _clientController.Clients;

    public IReadOnlyCollection<GameServer> Servers => _gameServerController.Servers;

    /// <inheritdoc />
    public Task Task => Task.WhenAll(_metaServices.Select(service => service.Task).Append(_clientListener.Task));

    /// <inheritdoc />
    public void Start() {
      Logger.Info("Starting ConnectServer...");
      _clientListener.Start();
      foreach (var service in _metaServices) {
        service.Start();
      }
      _isRunning = true;
      Logger.Info("Server successfully started");
    }

    /// <inheritdoc />
    public void Stop() {
      Logger.Info("Stopping ConnectServer...");
      _clientListener.Stop();
      foreach (var service in _metaServices) {
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

      (_clientListener as IDisposable)?.Dispose();
      (_clientController as IDisposable)?.Dispose();
      foreach (var service in _metaServices.OfType<IDisposable>()) {
        service.Dispose();
      }
    }

    private void OnBeforeClientAccepted(object sender, BeforeClientAcceptEventArgs ev) =>
      ev.RejectClient = _connectPlugins.Any(plugin => !plugin.OnAllowClientSocketAccept(ev.ClientSocket));
  }
}