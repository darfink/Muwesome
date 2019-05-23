using System;
using System.Net;
using System.Net.Sockets;
using log4net;

namespace Muwesome.Network.Tcp.Filters {
  /// <summary>A filter limiting the maximum amount of connections.</summary>
  public class MaxConnectionsFilter : IClientSocketFilter {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(MaxConnectionsFilter));
    private readonly int maxConnections;
    private int clientsConnected;

    /// <summary>Initializes a new instance of the <see cref="MaxConnectionsFilter"/> class.</summary>
    public MaxConnectionsFilter(int maxConnections) => this.maxConnections = maxConnections;

    /// <inheritdoc />
    public void Register<T>(IClientTcpListener<T> clientListener) {
      clientListener.ClientConnectionEstablished += this.OnClientConnectionEstablished;
      clientListener.ClientAccept += this.OnClientAccept;
    }

    private void OnClientAccept(object sender, ClientSocketAcceptEventArgs ev) {
      if (ev.RejectClient) {
        return;
      }

      if (this.clientsConnected >= this.maxConnections) {
        var ipAddress = ev.ClientSocket.RemoteEndPoint;
        Logger.WarnFormat("Connection refused from {0}; maximum server connections ({1}) reached", ipAddress, this.maxConnections);
        ev.RejectClient = true;
      }
    }

    private void OnClientConnectionEstablished(object sender, ClientConnectionEstablishedEventArgs ev) {
      this.clientsConnected++;
      ev.EstablishedConnection.Disconnected += (_, e) => this.clientsConnected--;
    }
  }
}