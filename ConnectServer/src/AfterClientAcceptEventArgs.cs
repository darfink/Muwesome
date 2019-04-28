using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Muwesome.Network;

namespace Muwesome.ConnectServer {
  public class AfterClientAcceptEventArgs : EventArgs {
    /// <summary>Initializes a new instance of the <see cref="AfterClientAcceptEventArgs"/> class.</summary>
    public AfterClientAcceptEventArgs(IConnection connection) => this.ClientConnection = connection;

    /// <summary>Gets the client connection.</summary>
    public IConnection ClientConnection { get; }
  }
}