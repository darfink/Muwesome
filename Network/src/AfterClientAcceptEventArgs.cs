using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Muwesome.Network;

namespace Muwesome.Network {
  public class AfterClientAcceptEventArgs : EventArgs {
    /// <summary>Initializes a new instance of the <see cref="AfterClientAcceptEventArgs"/> class.</summary>
    public AfterClientAcceptEventArgs(IPEndPoint localEndPoint, IConnection connection) {
      this.ClientConnection = connection;
      this.LocalEndPoint = localEndPoint;
    }

    /// <summary>Gets the client connection.</summary>
    public IConnection ClientConnection { get; }

    /// <summary>Gets the local end point the client connected to.</summary>
    public IPEndPoint LocalEndPoint { get; }
  }
}