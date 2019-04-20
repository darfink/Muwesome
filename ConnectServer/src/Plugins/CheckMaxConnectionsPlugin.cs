using System;
using System.Net;

namespace Muwesome.ConnectServer.Plugins {
  internal class CheckMaxConnectionsPlugin {
    private readonly IClientsController _clientsController;
    private readonly int _maxConnections;

    public CheckMaxConnectionsPlugin(IClientsController clientsController, int maxConnections) {
      _clientsController = clientsController;
      _clientsController.BeforeClientAccepted += OnBeforeClientAccepted;
      _maxConnections = maxConnections;
    }

    private void OnBeforeClientAccepted(object sender, BeforeClientAcceptEventArgs ev) {
      if (_clientsController.Clients.Count >= _maxConnections) {
        // TODO: Log about dis shit mannisch
        ev.RejectClient = true;
      }
    }
  }
}