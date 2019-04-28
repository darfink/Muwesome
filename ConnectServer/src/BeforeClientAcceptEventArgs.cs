using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Muwesome.Network;

namespace Muwesome.ConnectServer {
  public class BeforeClientAcceptEventArgs : EventArgs {
    /// <summary>Initializes a new instance of the <see cref="BeforeClientAcceptEventArgs"/> class.</summary>
    public BeforeClientAcceptEventArgs(Socket socket) => this.ClientSocket = socket;

    /// <summary>Gets or sets a value indicating whether this client will be rejected.</summary>
    public bool RejectClient { get; set; }

    /// <summary>Gets the client's socket.</summary>
    public Socket ClientSocket { get; }
  }
}