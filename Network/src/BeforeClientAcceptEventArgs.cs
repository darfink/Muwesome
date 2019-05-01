using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Muwesome.Network;

namespace Muwesome.Network {
  public class BeforeClientAcceptEventArgs : EventArgs {
    /// <summary>Initializes a new instance of the <see cref="BeforeClientAcceptEventArgs"/> class.</summary>
    public BeforeClientAcceptEventArgs(IPEndPoint localEndPoint, Socket socket) {
      this.ClientSocket = socket;
      this.LocalEndPoint = localEndPoint;
    }

    /// <summary>Gets the client's socket.</summary>
    public Socket ClientSocket { get; }

    /// <summary>Gets the local end point the client connected to.</summary>
    public IPEndPoint LocalEndPoint { get; }

    /// <summary>Gets or sets a value indicating whether this client will be rejected.</summary>
    public bool RejectClient { get; set; }
  }
}