using System;
using System.Net;
using System.Net.Sockets;
using log4net;

namespace Muwesome.Network.Tcp.Filters {
  /// <summary>A filter limiting the maximum amount of connections.</summary>
  public class MaxConnectionsFilter {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(MaxConnectionsFilter));
    private readonly int maxConnections;
    private int clientsConnected;

    /// <summary>Initializes a new instance of the <see cref="MaxConnectionsFilter"/> class.</summary>
    public MaxConnectionsFilter(IClientTcpListener clientListener, int maxConnections) {
      this.maxConnections = maxConnections;
      clientListener.ClientAccept += this.OnClientAccept;
      clientListener.ClientConnected += this.OnClientConnected;
    }

    private void OnClientAccept(object sender, ClientAcceptEventArgs ev) {
      if (ev.RejectClient) {
        return;
      }

      if (this.clientsConnected >= this.maxConnections) {
        var ipAddress = ev.ClientSocket.RemoteEndPoint;
        Logger.Warn($"Connection refused from {ipAddress}; maximum server connections ({this.maxConnections}) reached");
        ev.RejectClient = true;
      }
    }

    private void OnClientConnected(object sender, ClientConnectedEventArgs ev) {
      this.clientsConnected++;
      ev.ClientConnection.Disconnected += (_, e) => this.clientsConnected--;
    }
  }
}