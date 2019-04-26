using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Muwesome.Network;

namespace Muwesome.ConnectServer {
  public interface IClientListener : ILifecycle {
    /// <summary>An event that is raised before a client is accepted.</summary>
    event EventHandler<BeforeClientAcceptEventArgs> BeforeClientAccepted;

    /// <summary>An event that is raised after a client has been accepted.</summary>
    event EventHandler<AfterClientAcceptEventArgs> AfterClientAccepted;

    /// <summary>Gets whether the listener is bound or not.</summary>
    bool IsBound { get; }
  }

  public class BeforeClientAcceptEventArgs : EventArgs {
    /// <summary>Constructs a new instance of <see cref="BeforeClientAcceptEventArgs" />.</summary>
    public BeforeClientAcceptEventArgs(Socket socket) => ClientSocket = socket;

    /// <summary>Gets or sets whether this client will be rejected.</summary>
    public bool RejectClient { get; set; }

    /// <summary>Gets the client's socket.</summary>
    public Socket ClientSocket { get; }
  }

  public class AfterClientAcceptEventArgs : EventArgs {
    /// <summary>Constructs a new instance of <see cref="AfterClientAcceptEventArgs" />.</summary>
    public AfterClientAcceptEventArgs(IConnection connection) => ClientConnection = connection;

    /// <summary>Get's the client connection.</summary>
    public IConnection ClientConnection { get; }
  }
}