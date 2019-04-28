using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using log4net;

namespace Muwesome.ConnectServer.Plugins {
  internal class CheckMaxConnectionsPerIpPlugin : IConnectPlugin {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(CheckMaxConnectionsPerIpPlugin));
    private readonly ConcurrentDictionary<IPAddress, uint> ipAddressConnections;
    private readonly int maxConnectionsPerIp;

    /// <summary>Initializes a new instance of the <see cref="CheckMaxConnectionsPerIpPlugin"/> class.</summary>
    public CheckMaxConnectionsPerIpPlugin(IClientController clientsController, int maxConnectionsPerIp) {
      this.ipAddressConnections = new ConcurrentDictionary<IPAddress, uint>();
      this.maxConnectionsPerIp = maxConnectionsPerIp;
      clientsController.ClientSessionStarted += this.OnClientSessionStarted;
      clientsController.ClientSessionEnded += this.OnClientSessionEnded;
    }

    /// <inheritdoc />
    public bool OnAllowClientSocketAccept(Socket socket) {
      var ipAddress = this.GetIpAddressFromEndPoint(socket.RemoteEndPoint);
      this.ipAddressConnections.TryGetValue(ipAddress, out uint connectionsWithIp);

      if (connectionsWithIp >= this.maxConnectionsPerIp) {
        Logger.Warn($"Connection refused from {ipAddress}; maximum connections for IP ({this.maxConnectionsPerIp}) reached");
        return false;
      }

      return true;
    }

    private void OnClientSessionStarted(object sender, ClientSessionEventArgs ev) {
      var ipAddress = this.GetIpAddressFromEndPoint(ev.Client.Connection.RemoteEndPoint);
      this.ipAddressConnections.AddOrUpdate(ipAddress, 1, (_, count) => count + 1);
    }

    private void OnClientSessionEnded(object sender, ClientSessionEventArgs ev) {
      var ipAddress = this.GetIpAddressFromEndPoint(ev.Client.Connection.RemoteEndPoint);
      this.ipAddressConnections.AddOrUpdate(ipAddress, 0, (_, count) => count - 1);
    }

    private IPAddress GetIpAddressFromEndPoint(EndPoint endPoint) => ((IPEndPoint)endPoint).Address;
  }
}