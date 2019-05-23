using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using log4net;

namespace Muwesome.Network.Tcp.Filters {
  /// <summary>A filter limiting the maximum amount of connections per IP.</summary>
  public class MaxConnectionsPerIpFilter : IClientSocketFilter {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(MaxConnectionsPerIpFilter));
    private readonly ConcurrentDictionary<IPAddress, uint> ipAddressConnections = new ConcurrentDictionary<IPAddress, uint>();
    private readonly int maxConnectionsPerIp;

    /// <summary>Initializes a new instance of the <see cref="MaxConnectionsPerIpFilter"/> class.</summary>
    public MaxConnectionsPerIpFilter(int maxConnectionsPerIp) => this.maxConnectionsPerIp = maxConnectionsPerIp;

    /// <inheritdoc />
    public void Register<T>(IClientTcpListener<T> clientListener) {
      clientListener.ClientConnectionEstablished += this.OnClientConnectionEstablished;
      clientListener.ClientAccept += this.OnClientAccept;
    }

    private void OnClientAccept(object sender, ClientSocketAcceptEventArgs ev) {
      if (ev.RejectClient) {
        return;
      }

      var ipAddress = this.GetIpAddressFromEndPoint(ev.ClientSocket.RemoteEndPoint);
      this.ipAddressConnections.TryGetValue(ipAddress, out uint connectionsWithIp);

      if (connectionsWithIp >= this.maxConnectionsPerIp) {
        Logger.WarnFormat("Connection refused from {0}; maximum connections for IP ({1}) reached", ipAddress, this.maxConnectionsPerIp);
        ev.RejectClient = true;
      }
    }

    private void OnClientConnectionEstablished(object sender, ClientConnectionEstablishedEventArgs ev) {
      var ipAddress = this.GetIpAddressFromEndPoint(ev.EstablishedConnection.RemoteEndPoint);
      this.ipAddressConnections.AddOrUpdate(ipAddress, 1, (_, count) => count + 1);

      ev.EstablishedConnection.Disconnected += (s, e) => {
        var address = this.GetIpAddressFromEndPoint(ev.EstablishedConnection.RemoteEndPoint);
        this.ipAddressConnections.AddOrUpdate(address, 0, (_, count) => count - 1);
      };
    }

    private IPAddress GetIpAddressFromEndPoint(EndPoint endPoint) => ((IPEndPoint)endPoint).Address;
  }
}