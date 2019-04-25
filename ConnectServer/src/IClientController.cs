using System;
using System.Collections.Generic;

namespace Muwesome.ConnectServer {
  public interface IClientController {
    /// <summary>An event that is raised when a new client session started.</summary>
    event EventHandler<ClientSessionEventArgs> ClientSessionStarted;

    /// <summary>An event that is raised when a client session is ended.</summary>
    event EventHandler<ClientSessionEventArgs> ClientSessionEnded;

    /// <summary>Gets a list of connected clients.</summary>
    IReadOnlyCollection<Client> Clients { get; }

    /// <summary>Adds a new client to the session.</summary>
    void AddClient(Client client);
  }

  public class ClientSessionEventArgs : EventArgs {
    /// <summary>Constructs a new instance of <see cref="ClientSessionEventArgs" />.</summary>
    public ClientSessionEventArgs(Client client) => Client = client;

    /// <summary>Gets the client instance.</summary>
    public Client Client { get; }
  }
}