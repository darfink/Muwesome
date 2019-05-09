using System;
using System.Net.Sockets;

namespace Muwesome.Network.Tcp {
  public class ClientAcceptEventArgs : EventArgs {
    /// <summary>Initializes a new instance of the <see cref="ClientAcceptEventArgs"/> class.</summary>
    public ClientAcceptEventArgs(Socket socket) => this.ClientSocket = socket;

    /// <summary>Gets the client's socket.</summary>
    public Socket ClientSocket { get; }

    /// <summary>Gets or sets a value indicating whether this client will be rejected.</summary>
    public bool RejectClient { get; set; }
  }
}