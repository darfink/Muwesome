using System;
using System.Net;
using System.Net.Sockets;
using log4net;

namespace Muwesome.ConnectServer.Filters {
  internal class MaxConnectionsFilter : IClientConnectFilter {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(MaxConnectionsFilter));
    private readonly IClientController clientController;
    private readonly int maxConnections;

    /// <summary>Initializes a new instance of the <see cref="MaxConnectionsFilter"/> class.</summary>
    public MaxConnectionsFilter(IClientController clientController, int maxConnections) {
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