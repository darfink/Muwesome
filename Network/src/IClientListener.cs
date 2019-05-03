using System;
using Muwesome.Interfaces;

namespace Muwesome.Network {
  // TODO: IPs and sockets are exposed by the interface
  public interface IClientListener : ILifecycle {
    /// <summary>An event that is raised before a client is accepted.</summary>
    event EventHandler<BeforeClientAcceptEventArgs> BeforeClientAccepted;

    /// <summary>An event that is raised after a client has been accepted.</summary>
    event EventHandler<AfterClientAcceptEventArgs> AfterClientAccepted;
  }
}