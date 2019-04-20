using System;
using System.Net.Sockets;

namespace Muwesome.ConnectServer {
  internal class BeforeClientAcceptEventArgs : EventArgs {
    /// <summary>Constructs a new instance of <see cref="BeforeClientAcceptEventArgs" />.</summary>
    public BeforeClientAcceptEventArgs(Socket socket) => ClientSocket = socket;

    /// <summary>Gets or sets whether this client will be rejected.</summary>
    public bool RejectClient { get; set; }

    /// <summary>Gets the client's socket.</summary>
    public Socket ClientSocket { get; }
  }

  internal class AfterClientAcceptEventArgs : EventArgs {
    /// <summary>Constructs a new instance of <see cref="AfterClientAcceptEventArgs" />.</summary>
    public AfterClientAcceptEventArgs(Client client) => AcceptedClient = client;

    /// <summary>Get's the accepted client.</summary>
    public Client AcceptedClient { get; }
  }

  internal class AfterClientDisconnectEventArgs : EventArgs {
    /// <summary>Constructs a new instance of <see cref="AfterClientDisconnectEventArgs" />.</summary>
    public AfterClientDisconnectEventArgs(Client client) => DisconnectedClient = client;

    /// <summary>Get's the disconnected client.</summary>
    public Client DisconnectedClient { get; }
  }
}