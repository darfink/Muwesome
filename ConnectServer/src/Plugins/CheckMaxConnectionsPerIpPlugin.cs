using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Concurrent;
using log4net;

namespace Muwesome.ConnectServer.Plugins {
  internal class CheckMaxConnectionsPerIpPlugin : IConnectPlugin {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(CheckMaxConnectionsPerIpPlugin));
    private readonly ConcurrentDictionary<IPAddress, uint> _ipAddressConnections;
    private readonly int _maxConnectionsPerIp;

    public CheckMaxConnectionsPerIpPlugin(IClientController clientsController, int maxConnectionsPerIp) {
      _ipAddressConnections = new ConcurrentDictionary<IPAddress, uint>();
      _maxConnectionsPerIp = maxConnectionsPerIp;
      clientsController.ClientSessionStarted += OnClientSessionStarted;
      clientsController.ClientSessionEnded += OnClientSessionEnded;
    }

    public bool OnAllowClientSocketAccept(Socket socket) {
      var ipAddress = GetIpAddressFromEndPoint(socket.RemoteEndPoint);
      _ipAddressConnections.TryGetValue(ipAddress, out uint connectionsWithIp);

      if (connectionsWithIp >= _maxConnectionsPerIp) {
        Logger.Warn($"Connection refused from {ipAddress}; maximum connections for IP ({_maxConnectionsPerIp}) reached");
        return false;
      }

      return true;
    }

    private void OnClientSessionStarted(object sender, ClientSessionEventArgs ev) {
      var ipAddress = GetIpAddressFromEndPoint(ev.Client.Connection.RemoteEndPoint);
      _ipAddressConnections.AddOrUpdate(ipAddress, 1, (_, count) => count + 1);
    }

    private void OnClientSessionEnded(object sender, ClientSessionEventArgs ev) {
      var ipAddress = GetIpAddressFromEndPoint(ev.Client.Connection.RemoteEndPoint);
      _ipAddressConnections.AddOrUpdate(ipAddress, 0, (_, count) => count - 1);
    }

    private IPAddress GetIpAddressFromEndPoint(EndPoint endPoint) => ((IPEndPoint)endPoint).Address;
  }
}