using System;
using System.Net;
using System.Net.Sockets;
using log4net;

namespace Muwesome.ConnectServer.Plugins {
  internal class CheckMaxConnectionsPlugin : IConnectPlugin {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(CheckMaxConnectionsPlugin));
    private readonly IClientController clientController;
    private readonly int maxConnections;

    /// <summary>Initializes a new instance of the <see cref="CheckMaxConnectionsPlugin"/> class.</summary>
    public CheckMaxConnectionsPlugin(IClientController clientController, int maxConnections) {
      this.clientController = clientController;
      this.maxConnections = maxConnections;
    }

    /// <inheritdoc />
    public bool OnAllowClientSocketAccept(Socket socket) {
      if (this.clientController.Clients.Count >= this.maxConnections) {
        var ipAddress = socket.RemoteEndPoint;
        Logger.Warn($"Connection refused from {ipAddress}; maximum server connections ({this.maxConnections}) reached");
        return false;
      }

      return true;
    }
  }
}