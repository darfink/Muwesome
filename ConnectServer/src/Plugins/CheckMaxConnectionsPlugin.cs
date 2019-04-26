using System;
using System.Net;
using System.Net.Sockets;
using log4net;

namespace Muwesome.ConnectServer.Plugins {
  internal class CheckMaxConnectionsPlugin : IConnectPlugin {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(CheckMaxConnectionsPlugin));
    private readonly IClientController _clientController;
    private readonly int _maxConnections;

    public CheckMaxConnectionsPlugin(IClientController clientController, int maxConnections) {
      _clientController = clientController;
      _maxConnections = maxConnections;
    }

    /// <inheritdoc />
    public bool OnAllowClientSocketAccept(Socket socket) {
      if (_clientController.Clients.Count >= _maxConnections) {
        var ipAddress = socket.RemoteEndPoint;
        Logger.Warn($"Connection refused from {ipAddress}; maximum server connections ({_maxConnections}) reached");
        return false;
      }

      return true;
    }
  }
}