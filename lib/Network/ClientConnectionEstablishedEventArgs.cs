using System;
using System.Net;

namespace Muwesome.Network {
  public class ClientConnectionEstablishedEventArgs : EventArgs {
    /// <summary>Initializes a new instance of the <see cref="ClientConnectionEstablishedEventArgs"/> class.</summary>
    public ClientConnectionEstablishedEventArgs(IConnection connection) => this.EstablishedConnection = connection;

    /// <summary>Gets the connected client.</summary>
    public IConnection EstablishedConnection { get; }
  }
}