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
  public class ConnectServer : IDisposable {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(ConnectServer));
    private readonly IClientController _clientController;
    private readonly IClientListener _clientListener;
    private readonly IConnectPlugin[] _connectPlugins;
    private readonly Stopwatch _startTime;

    public ConnectServer(
        Configuration config,
        IClientController clientController,
        IClientListener clientListener,
        IPacketHandler<Client> clientProtocol
    ) {
      Config = config;
      _startTime = new Stopwatch();
      _startTime.Start();
      _clientListener = clientListener;
      _clientListener.BeforeClientAccepted += OnBeforeClientAccepted;
      _clientListener.AfterClientAccepted += (_clientController, ev) =>
        clientController.AddClient(new Client(ev.ClientConnection, clientProtocol));
      _clientController = clientController;
      _connectPlugins = new IConnectPlugin[] {
        new CheckMaxConnectionsPlugin(_clientController, config.MaxConnections),
        new CheckMaxConnectionsPerIpPlugin(_clientController, config.MaxConnectionsPerIp),
      };
    }

    public Configuration Config { get; }

    public TimeSpan Uptime => _startTime.Elapsed;

    public IReadOnlyCollection<Client> Clients => _clientController.Clients;

    public Task Task => _clientListener.Task;

    public void Start() {
      Logger.Info("Starting ConnectServer...");
      _clientListener.Start();
      Logger.Info("Server successfully started");
    }

    public void Stop() {
      Logger.Info("Stopping ConnectServer...");
      _clientListener.Stop();
      Logger.Info("Server stopped");
    }

    public void Dispose() {
      (_clientListener as IDisposable)?.Dispose();
      (_clientController as IDisposable)?.Dispose();
    }

    private void OnBeforeClientAccepted(object sender, BeforeClientAcceptEventArgs ev) =>
      ev.RejectClient = _connectPlugins.Any(plugin => !plugin.OnAllowClientSocketAccept(ev.ClientSocket));
  }
}