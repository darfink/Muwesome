using System;
using System.Collections.Generic;
using Muwesome.GameLogic;

namespace Muwesome.GameServer {
  /// <summary>A controller for clients.</summary>
  internal interface IClientController {
    /// <summary>An event that is raised when a new client session started.</summary>
    event EventHandler<ClientSessionEventArgs> ClientSessionStarted;

    /// <summary>An event that is raised when a client session is ended.</summary>
    event EventHandler<ClientSessionEventArgs> ClientSessionEnded;

    /// <summary>Gets the number of connected clients.</summary>
    int ClientsConnected { get; }

    /// <summary>Adds a new client to the session.</summary>
    void AddClient(Client client);

    /// <summary>Gets a client from the player instance.</summary>
    Client GetClientByPlayer(Player player);
  }

  /// <summary>Arguments for a client session event.</summary>
  internal class ClientSessionEventArgs : EventArgs {
    /// <summary>Initializes a new instance of the <see cref="ClientSessionEventArgs"/> class.</summary>
    public ClientSessionEventArgs(Client client) => this.Client = client;

    /// <summary>Gets the client instance.</summary>
    public Client Client { get; }
  }
}