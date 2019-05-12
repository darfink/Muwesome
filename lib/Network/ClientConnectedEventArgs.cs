using System;
using System.Net;

namespace Muwesome.Network {
  public class ClientConnectedEventArgs<TClient> : EventArgs {
    /// <summary>Initializes a new instance of the <see cref="ClientConnectedEventArgs"/> class.</summary>
    public ClientConnectedEventArgs(TClient client) => this.ConnectedClient = client;

    /// <summary>Gets the connected client.</summary>
    public TClient ConnectedClient { get; }
  }
}