using System;
using System.Net;

namespace Muwesome.Network.Tcp {
  public interface IClientTcpListener : IClientListener {
    /// <summary>An event that is raised before a client is accepted.</summary>
    event EventHandler<ClientAcceptEventArgs> ClientAccept;

    /// <summary>Gets the original end point.</summary>
    IPEndPoint SourceEndPoint { get; }

    /// <summary>Gets the bound end point.</summary>
    IPEndPoint BoundEndPoint { get; }
  }
}