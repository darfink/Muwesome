using System;
using System.Net;

namespace Muwesome.Network {
  public class ClientConnectedEventArgs : EventArgs {
    /// <summary>Initializes a new instance of the <see cref="ClientConnectedEventArgs"/> class.</summary>
    public ClientConnectedEventArgs(IConnection clientConnection) => this.ClientConnection = clientConnection;

    /// <summary>Gets the client connection.</summary>
    public IConnection ClientConnection { get; }
  }
}