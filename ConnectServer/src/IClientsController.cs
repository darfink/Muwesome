using System;
using System.Collections.Generic;

namespace Muwesome.ConnectServer {
  internal interface IClientsController {
    /// <summary>An event that is raised before a client is accepted.</summary>
    event EventHandler<BeforeClientAcceptEventArgs> BeforeClientAccepted;

    /// <summary>An event that is raised after a client has been accepted.</summary>
    event EventHandler<AfterClientAcceptEventArgs> AfterClientAccepted;

    /// <summary>An event that is raised after a client has disconnected.</summary>
    event EventHandler<AfterClientDisconnectEventArgs> AfterClientDisconnected;

    /// <summary>Gets a list of connected clients.</summary>
    IReadOnlyCollection<Client> Clients { get; }
  }
}