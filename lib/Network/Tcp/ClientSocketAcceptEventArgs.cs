using System;
using System.Net.Sockets;

namespace Muwesome.Network.Tcp {
  public class ClientSocketAcceptEventArgs : EventArgs {
    /// <summary>Initializes a new instance of the <see cref="ClientSocketAcceptEventArgs"/> class.</summary>
    public ClientSocketAcceptEventArgs(Socket socket) => this.ClientSocket = socket;

    /// <summary>Gets the client's socket.</summary>
    public Socket ClientSocket { get; }

    /// <summary>Gets or sets a value indicating whether this client will be rejected.</summary>
    public bool RejectClient { get; set; }
  }
}