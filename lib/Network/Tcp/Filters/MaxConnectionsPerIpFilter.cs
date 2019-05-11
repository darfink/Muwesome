using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using log4net;

namespace Muwesome.Network.Tcp.Filters {
  /// <summary>A filter limiting the maximum amount of connections per IP.</summary>
  public class MaxConnectionsPerIpFilter {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(MaxConnectionsPerIpFilter));
    private readonly ConcurrentDictionary<IPAddress, uint> ipAddressConnections = new ConcurrentDictionary<IPAddress, uint>();
    private readonly int maxConnectionsPerIp;

    /// <summary>Initializes a new instance of the <see cref="MaxConnectionsPerIpFilter"/> class.</summary>
    public MaxConnectionsPerIpFilter(IClientTcpListener clientListener, int maxConnectionsPerIp) {
      this.maxConnectionsPerIp = maxConnectionsPerIp;
      clientListener.ClientAccept += this.OnClientAccept;
      clientListener.ClientConnected += this.OnClientConnected;
    }

    private void OnClientAccept(object sender, ClientAcceptEventArgs ev) {
      if (ev.RejectClient) {
        return;
      }

      var ipAddress = this.GetIpAddressFromEndPoint(ev.ClientSocket.RemoteEndPoint);
      this.ipAddressConnections.TryGetValue(ipAddress, out uint connectionsWithIp);

      if (connectionsWithIp >= this.maxConnectionsPerIp) {
        Logger.Warn($"Connection refused from {ipAddress}; maximum connections for IP ({this.maxConnectionsPerIp}) reached");
        ev.RejectClient = true;
      }
    }

    private void OnClientConnected(object sender, ClientConnectedEventArgs ev) {
      var ipAddress = this.GetIpAddressFromEndPoint(ev.ClientConnection.RemoteEndPoint);
      this.ipAddressConnections.AddOrUpdate(ipAddress, 1, (_, count) => count + 1);

      ev.ClientConnection.Disconnected += (s, e) => {
        var address = this.GetIpAddressFromEndPoint(ev.ClientConnection.RemoteEndPoint);
        this.ipAddressConnections.AddOrUpdate(address, 0, (_, count) => count - 1);
      };
    }

    private IPAddress GetIpAddressFromEndPoint(EndPoint endPoint) => ((IPEndPoint)endPoint).Address;
  }
}