using System;
using System.Net;
using System.Collections.Concurrent;

namespace Muwesome.ConnectServer.Plugins {
  internal class CheckMaxConnectionsPerIpPlugin {
    private readonly ConcurrentDictionary<IPAddress, uint> _ipAddressConnections;
    private readonly int _maxConnectionsPerIp;

    public CheckMaxConnectionsPerIpPlugin(IClientsController clientsController, int maxConnectionsPerIp) {
      _ipAddressConnections = new ConcurrentDictionary<IPAddress, uint>();
      _maxConnectionsPerIp = maxConnectionsPerIp;

      clientsController.AfterClientAccepted += OnAfterClientAccepted;
      clientsController.BeforeClientAccepted += OnBeforeClientAccepted;
      clientsController.AfterClientDisconnected += OnAfterClientDisconnected;
    }

    private void OnBeforeClientAccepted(object sender, BeforeClientAcceptEventArgs ev) {
      var ipAddress = GetIpAddressFromEndPoint(ev.ClientSocket.RemoteEndPoint);
      _ipAddressConnections.TryGetValue(ipAddress, out uint connectionsWithIp);

      if (connectionsWithIp >= _maxConnectionsPerIp) {
        // TODO: LOG DAT SHIT
        ev.RejectClient = true;
      }
    }

    private void OnAfterClientAccepted(object sender, AfterClientAcceptEventArgs ev) {
      var ipAddress = GetIpAddressFromEndPoint(ev.AcceptedClient.Connection.RemoteEndPoint);
      _ipAddressConnections.AddOrUpdate(ipAddress, 1, (_, count) => count + 1);
    }

    private void OnAfterClientDisconnected(object sender, AfterClientDisconnectEventArgs ev) {
      var ipAddress = GetIpAddressFromEndPoint(ev.DisconnectedClient.Connection.RemoteEndPoint);
      _ipAddressConnections.AddOrUpdate(ipAddress, 0, (_, count) => count - 1);
    }

    private IPAddress GetIpAddressFromEndPoint(EndPoint endPoint) => ((IPEndPoint)endPoint).Address;
  }
}