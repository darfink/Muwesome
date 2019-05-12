using System;
using System.Net;

namespace Muwesome.Network.Tcp {
  public interface IClientTcpListener<TClient> : IClientListener<TClient> {
    /// <summary>An event that is raised before a client socket is accepted.</summary>
    event EventHandler<ClientSocketAcceptEventArgs> ClientAccept;

    /// <summary>Gets the original end point.</summary>
    IPEndPoint SourceEndPoint { get; }

    /// <summary>Gets the bound end point.</summary>
    IPEndPoint BoundEndPoint { get; }
  }
}